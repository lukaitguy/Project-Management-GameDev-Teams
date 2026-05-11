import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ClanoviService } from '../../../core/services/clanovi.service';
import { AuthService } from '../../../core/services/auth.service';
import { ClanProjekta } from '../../../core/models/clan-projekta.model';

@Component({
  selector: 'app-project-members',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './project-members.component.html',
  styleUrl: './project-members.component.scss'
})
export class ProjectMembersComponent implements OnInit {

  @Input() projekatId!: string;
  @Output() clanoviLoaded = new EventEmitter<ClanProjekta[]>();

  clanovi: ClanProjekta[] = [];
  isAdmin = false;
  showForm = false;
  greska: string | null = null;
  uspeh: string | null = null;
  ucitavanje = true;

  uloge = ['Developer', 'Designer', 'Tester', 'Projektni Menadzer', 'Ostalo'];
  noviEmail = '';
  novaUloga = '';

  constructor(
    private clanoviService: ClanoviService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.isAdmin = this.authService.getCurrentUser()?.isAdmin ?? false;
    this.ucitajClanove();
  }

  ucitajClanove(): void {
    this.clanoviService.getByProjekatId(this.projekatId).subscribe({
      next: (data) => {
        this.clanovi = data;
        this.clanoviLoaded.emit(data);  // notify parent so tasks can use members
        this.ucitavanje = false;
      },
      error: () => {
        this.greska = 'Greška pri učitavanju članova.';
        this.ucitavanje = false;
      }
    });
  }

  dodajClana(): void {
    if (!this.noviEmail) {
      this.greska = 'Email je obavezan.';
      return;
    }

    this.clanoviService.add(this.projekatId, {
      email: this.noviEmail,
      uloga: this.novaUloga || null
    }).subscribe({
      next: () => {
        this.uspeh = 'Član uspešno dodat.';
        this.greska = null;
        this.noviEmail = '';
        this.novaUloga = '';
        this.showForm = false;
        this.ucitajClanove();
      },
      error: (err) => {
        this.greska = err.error?.message ?? 'Greška pri dodavanju člana.';
        this.uspeh = null;
      }
    });
  }

  ukloniClana(korisnikId: string): void {
    if (!confirm('Ukloniti člana sa projekta?')) return;
    this.clanoviService.remove(this.projekatId, korisnikId).subscribe({
      next: () => {
        this.clanovi = this.clanovi.filter(c => c.korisnikId !== korisnikId);
        this.clanoviLoaded.emit(this.clanovi);
      },
      error: () => this.greska = 'Greška pri uklanjanju člana.'
    });
  }

  toggleForm(): void {
    this.showForm = !this.showForm;
    this.greska = null;
    this.uspeh = null;
  }
}