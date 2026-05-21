// features/dashboard/task-details/task-details.component.ts
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
  currentUserId = '';

  noviKomentar = '';
  statusGreska = '';
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
    this.ucitajZadatak();
    this.ucitajKomentare();
  }

  ucitajZadatak(): void {
    this.zadaciService.getById(this.projekatId, this.zadatakId).subscribe({
      next: (res) => {
        this.zadatak = res;
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

  promeniStatus(noviStatus: string): void {
    this.statusGreska = '';
    this.zadaciService.updateStatus(this.projekatId, this.zadatakId, noviStatus).subscribe({
      next: () => {
        if (this.zadatak) this.zadatak.status = noviStatus;
      },
      error: (err) => {
        this.statusGreska = err.error?.message ?? 'Greška pri promeni statusa.';
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