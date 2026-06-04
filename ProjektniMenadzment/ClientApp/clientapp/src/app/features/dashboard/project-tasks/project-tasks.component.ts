import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ZadaciService } from '../../../core/services/zadaci.service';
import { KomentariService } from '../../../core/services/komentari.service';
import { AuthService } from '../../../core/services/auth.service';
import { Zadatak } from '../../../core/models/zadatak.model';
import { Komentar } from '../../../core/models/komentar.model';
import { CreateZadatak } from '../../../core/models/create-zadatak.model';
import { ClanProjekta } from '../../../core/models/clan-projekta.model';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-project-tasks',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './project-tasks.component.html',
  styleUrl: './project-tasks.component.scss'
})
export class ProjectTasksComponent implements OnInit {

  @Input() projekatId!: string;
  @Input() clanovi: ClanProjekta[] = [];

  zadaci: Zadatak[] = [];
  isAdmin = false;
  showForm = false;
  greska: string | null = null;
  ucitavanje = true;

  statusi = ['Nije zapocet', 'U toku', 'Pauziran', 'Otkazan', 'Zavrsen'];
  prioriteti = ['Nizak', 'Srednji', 'Visok'];
  tipoviZadatka = ['Programiranje', 'Dizajn', 'Testiranje', 'Dokumentacija', 'Ostalo'];

  form: CreateZadatak = this.initForm();

  // Comments state per task
  expandedTaskId: string | null = null;
  komentariMap: Record<string, Komentar[]> = {};
  noviKomentarMap: Record<string, string> = {};
  komentariUcitavanje: Record<string, boolean> = {};

  constructor(
    private zadaciService: ZadaciService,
    private komentariService: KomentariService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.isAdmin = this.authService.getCurrentUser()?.isAdmin ?? false;
    this.ucitajZadatke();
  }

  ucitajZadatke(): void {
    this.zadaciService.getByProjekatId(this.projekatId).subscribe({
      next: (data) => {
        this.zadaci = data;
        this.ucitavanje = false;
      },
      error: () => {
        this.greska = 'Greška pri učitavanju zadataka.';
        this.ucitavanje = false;
      }
    });
  }

  toggleTask(zadatakId: string): void {
    if (this.expandedTaskId === zadatakId) {
      this.expandedTaskId = null;
      return;
    }
    this.expandedTaskId = zadatakId;
    if (!this.komentariMap[zadatakId]) {
      this.ucitajKomentare(zadatakId);
    }
  }

  ucitajKomentare(zadatakId: string): void {
    this.komentariUcitavanje[zadatakId] = true;
    this.komentariService.getByZadatakId(this.projekatId, zadatakId).subscribe({
      next: (data) => {
        this.komentariMap[zadatakId] = data;
        this.komentariUcitavanje[zadatakId] = false;
      },
      error: () => {
        this.komentariMap[zadatakId] = [];
        this.komentariUcitavanje[zadatakId] = false;
      }
    });
  }

  dodajKomentar(zadatakId: string): void {
    const sadrzaj = this.noviKomentarMap[zadatakId]?.trim();
    if (!sadrzaj) return;

    this.komentariService.add(this.projekatId, zadatakId, sadrzaj).subscribe({
      next: (komentar) => {
        this.komentariMap[zadatakId] = [
          ...(this.komentariMap[zadatakId] || []),
          komentar
        ];
        this.noviKomentarMap[zadatakId] = '';
      },
      error: (err) => {
        this.greska = err.error?.message ?? 'Greška pri dodavanju komentara.';
      }
    });
  }

  sacuvajZadatak(): void {
    if (!this.form.naslov || !this.form.status || !this.form.prioritet) {
      this.greska = 'Naslov, status i prioritet su obavezni.';
      return;
    }
    this.zadaciService.create(this.projekatId, this.form).subscribe({
      next: () => {
        this.showForm = false;
        this.form = this.initForm();
        this.greska = null;
        this.ucitajZadatke();
      },
      error: (err) => {
        this.greska = err.error?.message ?? 'Greška pri kreiranju zadatka.';
      }
    });
  }

  promeniStatus(zadatak: Zadatak, noviStatus: string): void {
    this.zadaciService.updateStatus(this.projekatId, zadatak.id, noviStatus).subscribe({
      next: () => zadatak.status = noviStatus,
      error: (err) => this.greska = err.error?.message ?? 'Greška pri promeni statusa.'
    });
  }

  obrisiZadatak(id: string): void {
    if (!confirm('Obrisati zadatak?')) return;
    this.zadaciService.delete(this.projekatId, id).subscribe({
      next: () => {
        this.zadaci = this.zadaci.filter(z => z.id !== id);
        if (this.expandedTaskId === id) this.expandedTaskId = null;
      },
      error: () => this.greska = 'Greška pri brisanju zadatka.'
    });
  }

  toggleForm(): void {
    this.showForm = !this.showForm;
    this.greska = null;
    if (!this.showForm) this.form = this.initForm();
  }

  private initForm(): CreateZadatak {
    return {
      naslov: '',
      opis: null,
      status: 'Nije zapocet',
      prioritet: 'Srednji',
      tipZadatka: null,
      rok: null,
      dodeljenKorisnikuId: null
    };
  }
}