import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ProjekatIzvestaj } from '../../../core/models/projekat-izvestaj.model';
import { IzvestajiService } from '../../../core/services/izvestaji.service';

@Component({
  selector: 'app-izvestaj-projekta',
  imports: [CommonModule, RouterLink],
  templateUrl: './izvestaj-projekta.component.html',
  styleUrl: './izvestaj-projekta.component.scss'
})
export class IzvestajProjektaComponent implements OnInit {

  izvestaj?: ProjekatIzvestaj;
  loading = false;
  greska = '';

  constructor(
    private route: ActivatedRoute,
    private izvestajiService: IzvestajiService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.loading = true;

    this.izvestajiService.getProjekatIzvestaj(id).subscribe({
      next: (data) => {
        this.izvestaj = data;
      },
      error: (err) => {
        this.greska = err?.error?.message || 'Greska pri ucitavanju izvestaja.';
        this.loading = false;
      },
      complete: () => {
        this.loading = false;
      }
    });
  }

  get progressWidth(): string {
    return `${this.izvestaj?.postotakZavrsenosti ?? 0}%`;
  }

  get daysInfo(): string {
    if (!this.izvestaj?.rok) return '—';
    const rok = new Date(this.izvestaj.rok);
    const danas = new Date();
    danas.setHours(0, 0, 0, 0);
    const diff = Math.round((rok.getTime() - danas.getTime()) / (1000 * 60 * 60 * 24));
    if (diff < 0) return `${Math.abs(diff)} dana kasni`;
    if (diff === 0) return 'Danas ističe rok';
    return `${diff} dana do roka`;
  }

  get daysInfoClass(): string {
    if (!this.izvestaj?.rok) return '';
    const rok = new Date(this.izvestaj.rok);
    const danas = new Date();
    danas.setHours(0, 0, 0, 0);
    const diff = Math.round((rok.getTime() - danas.getTime()) / (1000 * 60 * 60 * 24));
    if (diff < 0) return 'late';
    if (diff <= 7) return 'soon';
    return 'ok';
  }

  formatDate(value?: string | null): string {
    if (!value) return '—';
    const d = new Date(value);
    return Number.isNaN(d.getTime()) ? '—' : d.toLocaleDateString('sr-RS');
  }

  formatCurrency(value?: number | null): string {
    if (value == null) return '—';
    return value.toLocaleString('sr-RS') + ' RSD';
  }
}
