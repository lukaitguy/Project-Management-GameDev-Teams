import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MojZadatak } from '../../../core/models/moj-zadatak.model';
import { ZadaciService } from '../../../core/services/zadaci.service';

@Component({
  selector: 'app-my-tasks',
  imports: [CommonModule, RouterLink],
  templateUrl: './my-tasks.component.html',
  styleUrl: './my-tasks.component.scss'
})
export class MyTasksComponent implements OnInit {

  zadaci: MojZadatak[] = [];
  loading = false;
  greska = '';

  constructor(private zadaciService: ZadaciService) {}

  ngOnInit(): void {
    this.ucitaj();
  }

  private ucitaj(): void {
    this.loading = true;
    this.greska = '';

    this.zadaciService.getMojiZadaci().subscribe({
      next: (data) => {
        this.zadaci = data;
      },
      error: (err) => {
        console.error(err);
        this.greska = err?.error?.message || 'Doslo je do greske pri ucitavanju zadataka.';
        this.loading = false;
      },
      complete: () => {
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
