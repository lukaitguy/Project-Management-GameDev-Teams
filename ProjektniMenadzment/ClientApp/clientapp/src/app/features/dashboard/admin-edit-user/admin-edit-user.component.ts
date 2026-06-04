import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AdminUpdateKorisnik } from '../../../core/models/admin-korisnik.model';
import { AdminKorisniciService } from '../../../core/services/admin-korisnici.service';

@Component({
  selector: 'app-admin-edit-user',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './admin-edit-user.component.html',
  styleUrl: './admin-edit-user.component.scss'
})
export class AdminEditUserComponent implements OnInit {

  identityUserId = '';
  roles: string[] = [];
  selectedRoles: string[] = [];

  form: AdminUpdateKorisnik = {
    userName: '',
    email: '',
    roles: [],
    ime: null,
    prezime: null,
    brojTelefona: null,
    biografija: null
  };

  novaLozinka = '';

  loading = false;
  loadingLozinka = false;
  greska = '';
  greskeLozinka = '';
  uspeh = '';
  uspehLozinka = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private adminService: AdminKorisniciService
  ) {}

  ngOnInit(): void {
    this.identityUserId = this.route.snapshot.paramMap.get('id')!;
    this.ucitajRoles();
    this.ucitajKorisnika();
  }

  private ucitajRoles(): void {
    this.adminService.getRoles().subscribe({
      next: (data) => this.roles = data,
      error: () => {}
    });
  }

  private ucitajKorisnika(): void {
    this.adminService.getById(this.identityUserId).subscribe({
      next: (k) => {
        this.form = {
          userName: k.userName,
          email: k.email,
          roles: [...k.roles],
          ime: k.ime ?? null,
          prezime: k.prezime ?? null,
          brojTelefona: k.brojTelefona ?? null,
          biografija: k.biografija ?? null
        };
        this.selectedRoles = [...k.roles];
      },
      error: (err) => {
        this.greska = err?.error?.message || 'Korisnik nije pronadjen.';
      }
    });
  }

  toggleRole(role: string): void {
    const idx = this.selectedRoles.indexOf(role);
    if (idx === -1) {
      this.selectedRoles = [...this.selectedRoles, role];
    } else {
      this.selectedRoles = this.selectedRoles.filter(r => r !== role);
    }
  }

  isRoleSelected(role: string): boolean {
    return this.selectedRoles.includes(role);
  }

  sacuvaj(): void {
    if (!this.form.userName.trim() || !this.form.email.trim()) {
      this.greska = 'Korisnicko ime i email su obavezni.';
      return;
    }

    this.greska = '';
    this.uspeh = '';
    this.loading = true;

    this.adminService.update(this.identityUserId, { ...this.form, roles: this.selectedRoles }).subscribe({
      next: () => {
        this.uspeh = 'Podaci su sacuvani.';
        this.loading = false;
      },
      error: (err) => {
        this.greska = err?.error?.message || 'Greska pri cuvanju.';
        this.loading = false;
      }
    });
  }

  promeniLozinku(): void {
    if (!this.novaLozinka.trim()) {
      this.greskeLozinka = 'Lozinka ne sme biti prazna.';
      return;
    }

    this.greskeLozinka = '';
    this.uspehLozinka = '';
    this.loadingLozinka = true;

    this.adminService.changePassword(this.identityUserId, this.novaLozinka).subscribe({
      next: () => {
        this.uspehLozinka = 'Lozinka je promenjena.';
        this.novaLozinka = '';
        this.loadingLozinka = false;
      },
      error: (err) => {
        this.greskeLozinka = err?.error?.message || 'Greska pri promeni lozinke.';
        this.loadingLozinka = false;
      }
    });
  }
}
