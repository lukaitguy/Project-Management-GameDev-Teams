import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { MojResurs } from '../../../core/models/moj-resurs.model';
import { MojZadatak } from '../../../core/models/moj-zadatak.model';
import { Projekat } from '../../../core/models/projekat.model';
import { AuthService } from '../../../core/services/auth.service';
import { ProjektiService } from '../../../core/services/projekti.service';
import { ResursiService } from '../../../core/services/resursi.service';
import { ZadaciService } from '../../../core/services/zadaci.service';

@Component({
  selector: 'app-home',
  imports: [CommonModule, RouterLink],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {

  projekti: Projekat[] = [];
  mojiZadaci: MojZadatak[] = [];
  mojiResursi: MojResurs[] = [];
  loading = false;
  greska = '';

  constructor(
    public authService: AuthService,
    private projektiService: ProjektiService,
    private zadaciService: ZadaciService,
    private resursiService: ResursiService
  ) {}

  get isAdmin(): boolean {
    return this.authService.getCurrentUser()?.isAdmin === true;
  }

  get korisnickoIme(): string {
    return this.authService.getCurrentUser()?.korisnickoIme ?? 'korisnice';
  }

  ngOnInit(): void {
    this.ucitaj();
  }

  private ucitaj(): void {
    this.loading = true;
    this.greska = '';

    forkJoin({
      projekti: this.projektiService.getMojiProjekti(),
      zadaci: this.zadaciService.getMojiZadaci(),
      resursi: this.resursiService.getMojiResursi()
    }).subscribe({
      next: ({ projekti, zadaci, resursi }) => {
        this.projekti = projekti;
        this.mojiZadaci = zadaci;
        this.mojiResursi = resursi;
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.greska = 'Doslo je do greske pri ucitavanju podataka.';
        this.loading = false;
      }
    });
  }

  formatDate(value?: string | null): string {
    if (!value) return '—';
    const d = new Date(value);
    return Number.isNaN(d.getTime()) ? '—' : d.toLocaleDateString('sr-RS');
  }
}
