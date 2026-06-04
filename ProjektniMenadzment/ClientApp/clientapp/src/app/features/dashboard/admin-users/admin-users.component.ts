import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AdminKorisnik } from '../../../core/models/admin-korisnik.model';
import { AdminKorisniciService } from '../../../core/services/admin-korisnici.service';

@Component({
  selector: 'app-admin-users',
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './admin-users.component.html',
  styleUrl: './admin-users.component.scss'
})
export class AdminUsersComponent implements OnInit {

  korisnici: AdminKorisnik[] = [];
  roles: string[] = [];
  loading = false;
  greska = '';
  uspeh = '';

  filterSearch = '';
  filterRole = '';

  constructor(private adminService: AdminKorisniciService) {}

  ngOnInit(): void {
    this.ucitajRoles();
    this.ucitaj();
  }

  ucitaj(): void {
    this.loading = true;
    this.greska = '';

    this.adminService.getAll(
      this.filterSearch.trim() || undefined,
      this.filterRole || undefined
    ).subscribe({
      next: (data) => {
        this.korisnici = data;
      },
      error: (err) => {
        this.greska = err?.error?.message || 'Greska pri ucitavanju korisnika.';
        this.loading = false;
      },
      complete: () => {
        this.loading = false;
      }
    });
  }

  private ucitajRoles(): void {
    this.adminService.getRoles().subscribe({
      next: (data) => this.roles = data,
      error: () => {}
    });
  }

  obrisi(identityUserId: string, userName: string): void {
    if (!confirm(`Obrisati korisnika "${userName}"? Ova akcija je nepovratna.`)) return;

    this.greska = '';
    this.uspeh = '';

    this.adminService.delete(identityUserId).subscribe({
      next: () => {
        this.korisnici = this.korisnici.filter(k => k.identityUserId !== identityUserId);
        this.uspeh = `Korisnik "${userName}" je obrisan.`;
      },
      error: (err) => {
        this.greska = err?.error?.message || 'Greska pri brisanju korisnika.';
      }
    });
  }

  roleLabel(roles: string[]): string {
    return roles.length > 0 ? roles.join(', ') : 'Bez uloge';
  }
}
