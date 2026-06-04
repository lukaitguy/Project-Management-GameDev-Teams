import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MojResurs } from '../../../core/models/moj-resurs.model';
import { ResursiService } from '../../../core/services/resursi.service';

@Component({
  selector: 'app-my-resources',
  imports: [CommonModule, RouterLink],
  templateUrl: './my-resources.component.html',
  styleUrl: './my-resources.component.scss'
})
export class MyResourcesComponent implements OnInit {

  resursi: MojResurs[] = [];
  loading = false;
  greska = '';

  constructor(private resursiService: ResursiService) {}

  ngOnInit(): void {
    this.ucitaj();
  }

  private ucitaj(): void {
    this.loading = true;
    this.greska = '';

    this.resursiService.getMojiResursi().subscribe({
      next: (data) => {
        this.resursi = data;
      },
      error: (err) => {
        console.error(err);
        this.greska = err?.error?.message || 'Doslo je do greske pri ucitavanju resursa.';
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
