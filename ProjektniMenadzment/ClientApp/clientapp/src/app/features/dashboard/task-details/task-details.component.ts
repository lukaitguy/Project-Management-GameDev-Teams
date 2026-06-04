import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ZadaciService } from '../../../core/services/zadaci.service';
import { KomentariService } from '../../../core/services/komentari.service';
import { AuthService } from '../../../core/services/auth.service';
import { ZadatakDetails } from '../../../core/models/zadatak-details.model';
import { Komentar } from '../../../core/models/komentar.model';

@Component({
  selector: 'app-task-details',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './task-details.component.html',
  styleUrl: './task-details.component.scss'
})
export class TaskDetailsComponent implements OnInit {

  projekatId = '';
  zadatakId = '';
  zadatak?: ZadatakDetails;
  komentari: Komentar[] = [];

  loading = true;
  greska = '';
  isAdmin = false;
  isPM = false;
  korisnikId: string | null = null;

  odabraniStatus = '';
  statusGreska = '';
  statusLoading = false;

  preuzimanje = false;
  preuzimGreska = '';

  odustajanje = false;
  odustaniGreska = '';

  noviKomentar = '';
  komentarGreska = '';

  statusi = ['Nije zapocet', 'U toku', 'Pauziran', 'Otkazan', 'Zavrsen'];

  constructor(
    private route: ActivatedRoute,
    private zadaciService: ZadaciService,
    private komentariService: KomentariService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.projekatId = this.route.snapshot.paramMap.get('id')!;
    this.zadatakId = this.route.snapshot.paramMap.get('zadatakId')!;
    const user = this.authService.getCurrentUser();
    this.isAdmin = user?.isAdmin ?? false;
    this.isPM = user?.isPM ?? false;
    this.korisnikId = user?.korisnikId ?? null;
    this.ucitajZadatak();
    this.ucitajKomentare();
  }

  get mozeDaUpravlja(): boolean {
    return this.isAdmin || this.isPM;
  }

  get jeDodeljenMeni(): boolean {
    return !!this.zadatak?.dodeljenKorisnikuId &&
           this.zadatak.dodeljenKorisnikuId === this.korisnikId;
  }

  get mozeDaPreuzme(): boolean {
    return !this.mozeDaUpravlja &&
           !this.zadatak?.dodeljenKorisnikuId &&
           this.zadatak?.status === 'Nije zapocet';
  }

  ucitajZadatak(): void {
    this.zadaciService.getById(this.projekatId, this.zadatakId).subscribe({
      next: (res) => {
        this.zadatak = res;
        this.odabraniStatus = res.status;
        this.loading = false;
      },
      error: () => {
        this.greska = 'Greška pri učitavanju zadatka.';
        this.loading = false;
      }
    });
  }

  ucitajKomentare(): void {
    this.komentariService.getByZadatakId(this.projekatId, this.zadatakId).subscribe({
      next: (res) => this.komentari = res,
      error: () => this.komentari = []
    });
  }

  promeniStatus(): void {
    if (!this.zadatak || this.odabraniStatus === this.zadatak.status) return;
    this.statusGreska = '';
    this.statusLoading = true;

    this.zadaciService.updateStatus(this.projekatId, this.zadatakId, this.odabraniStatus).subscribe({
      next: () => {
        if (this.zadatak) this.zadatak.status = this.odabraniStatus;
        this.statusLoading = false;
      },
      error: (err) => {
        this.statusGreska = err.error?.message ?? 'Greška pri promeni statusa.';
        this.statusLoading = false;
      }
    });
  }

  preuzmiZadatak(): void {
    this.preuzimanje = true;
    this.preuzimGreska = '';

    this.zadaciService.preuzmi(this.projekatId, this.zadatakId).subscribe({
      next: () => {
        this.ucitajZadatak();
        this.preuzimanje = false;
      },
      error: (err) => {
        this.preuzimGreska = err.error?.message ?? 'Greška pri preuzimanju zadatka.';
        this.preuzimanje = false;
      }
    });
  }

  odustaniOdZadatka(): void {
    if (!confirm('Odustati od zadatka? Zadatak će biti vraćen kao slobodan.')) return;
    this.odustajanje = true;
    this.odustaniGreska = '';

    this.zadaciService.odustani(this.projekatId, this.zadatakId).subscribe({
      next: () => {
        this.ucitajZadatak();
        this.odustajanje = false;
      },
      error: (err) => {
        this.odustaniGreska = err.error?.message ?? 'Greška pri odustajanju od zadatka.';
        this.odustajanje = false;
      }
    });
  }

  dodajKomentar(): void {
    if (!this.noviKomentar.trim()) return;
    this.komentarGreska = '';

    this.komentariService.add(this.projekatId, this.zadatakId, this.noviKomentar).subscribe({
      next: (komentar) => {
        this.komentari = [...this.komentari, komentar];
        this.noviKomentar = '';
      },
      error: (err) => {
        this.komentarGreska = err.error?.message ?? 'Greška pri dodavanju komentara.';
      }
    });
  }
}
