import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { BuildoviService } from '../../../core/services/buildovi.service';
import { Build } from '../../../core/models/build.model';

@Component({
  selector: 'app-project-builds',
  imports: [CommonModule, RouterLink],
  templateUrl: './project-builds.component.html',
  styleUrl: './project-builds.component.scss'
})
export class ProjectBuildsComponent implements OnInit {

  projekatId = '';
  buildovi: Build[] = [];
  pagedBuildovi: Build[] = [];

  loading = false;
  greska = '';

  currentPage = 1;
  pageSize = 3;
  totalPages = 0;

  constructor(
    private route: ActivatedRoute,
    private buildoviService: BuildoviService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) return;

    this.projekatId = id;
    this.ucitajBuildove();
  }

  ucitajBuildove(): void {
    this.loading = true;
    this.greska = '';

    this.buildoviService.getByProjekatId(this.projekatId).subscribe({
      next: (res) => {
        this.buildovi = res;
        this.totalPages = Math.ceil(this.buildovi.length / this.pageSize);
        this.prikaziStranicu(1);
      },
      error: (err) => {
        console.error(err);
        this.greska = 'Došlo je do greške pri učitavanju buildova.';
        this.loading = false;
      },
      complete: () => {
        this.loading = false;
      }
    });
  }

  prikaziStranicu(page: number): void {
    this.currentPage = page;

    const startIndex = (page - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;

    this.pagedBuildovi = this.buildovi.slice(startIndex, endIndex);
  }

  prethodnaStranica(): void {
    if (this.currentPage > 1) {
      this.prikaziStranicu(this.currentPage - 1);
    }
  }

  sledecaStranica(): void {
    if (this.currentPage < this.totalPages) {
      this.prikaziStranicu(this.currentPage + 1);
    }
  }

  get pageNumbers(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }
}