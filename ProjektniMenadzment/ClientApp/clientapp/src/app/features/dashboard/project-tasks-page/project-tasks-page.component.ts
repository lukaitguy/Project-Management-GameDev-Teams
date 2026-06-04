// features/dashboard/project-tasks-page/project-tasks-page.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ZadaciService } from '../../../core/services/zadaci.service';
import { AuthService } from '../../../core/services/auth.service';
import { Zadatak } from '../../../core/models/zadatak.model';

@Component({
  selector: 'app-project-tasks-page',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './project-tasks-page.component.html',
  styleUrl: './project-tasks-page.component.scss'
})
export class ProjectTasksPageComponent implements OnInit {

  projekatId = '';
  zadaci: Zadatak[] = [];
  filteredZadaci: Zadatak[] = [];
  pagedZadaci: Zadatak[] = [];

  loading = false;
  greska = '';
  isAdmin = false;
  isPM = false;

  get canManage(): boolean {
    return this.isAdmin || this.isPM;
  }

  filterStatus = '';
  filterPrioritet = '';

  statusi = ['Nije zapocet', 'U toku', 'Pauziran', 'Otkazan', 'Zavrsen'];
  prioriteti = ['Nizak', 'Srednji', 'Visok'];

  currentPage = 1;
  pageSize = 5;
  totalPages = 0;

  constructor(
    private route: ActivatedRoute,
    private zadaciService: ZadaciService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.projekatId = this.route.snapshot.paramMap.get('id')!;
    const user = this.authService.getCurrentUser();
    this.isAdmin = user?.isAdmin ?? false;
    this.isPM = user?.isPM ?? false;
    this.ucitajZadatke();
  }

  ucitajZadatke(): void {
    this.loading = true;
    this.greska = '';

    this.zadaciService.getByProjekatId(this.projekatId).subscribe({
      next: (res) => {
        this.zadaci = res;
        this.applyFilters();
      },
      error: () => {
        this.greska = 'Došlo je do greške pri učitavanju zadataka.';
        this.loading = false;
      },
      complete: () => this.loading = false
    });
  }

  applyFilters(): void {
    this.filteredZadaci = this.zadaci.filter(z => {
      const statusMatch = !this.filterStatus || z.status === this.filterStatus;
      const prioritetMatch = !this.filterPrioritet || z.prioritet === this.filterPrioritet;
      return statusMatch && prioritetMatch;
    });
    this.totalPages = Math.ceil(this.filteredZadaci.length / this.pageSize);
    this.prikaziStranicu(1);
  }

  prikaziStranicu(page: number): void {
    this.currentPage = page;
    const start = (page - 1) * this.pageSize;
    this.pagedZadaci = this.filteredZadaci.slice(start, start + this.pageSize);
  }

  prethodnaStranica(): void {
    if (this.currentPage > 1) this.prikaziStranicu(this.currentPage - 1);
  }

  sledecaStranica(): void {
    if (this.currentPage < this.totalPages) this.prikaziStranicu(this.currentPage + 1);
  }

  get pageNumbers(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  resetFilters(): void {
    this.filterStatus = '';
    this.filterPrioritet = '';
    this.applyFilters();
  }
}