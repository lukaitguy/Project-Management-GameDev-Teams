import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Projekat } from '../../../core/models/projekat.model';
import { ProjektiService } from '../../../core/services/projekti.service';

@Component({
  selector: 'app-izvestaji-projekti',
  imports: [CommonModule, RouterLink],
  templateUrl: './izvestaji-projekti.component.html',
  styleUrl: './izvestaji-projekti.component.scss'
})
export class IzvestajiProjektiComponent implements OnInit {

  projekti: Projekat[] = [];
  loading = false;
  greska = '';

  constructor(private projektiService: ProjektiService) {}

  ngOnInit(): void {
    this.loading = true;
    this.projektiService.getMojiProjekti().subscribe({
      next: (data) => {
        this.projekti = data;
      },
      error: (err) => {
        this.greska = err?.error?.message || 'Greska pri ucitavanju projekata.';
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
