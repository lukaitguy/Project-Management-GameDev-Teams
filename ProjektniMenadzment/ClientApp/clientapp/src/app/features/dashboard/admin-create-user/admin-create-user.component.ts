import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AdminCreateKorisnik } from '../../../core/models/admin-korisnik.model';
import { AdminKorisniciService } from '../../../core/services/admin-korisnici.service';

@Component({
  selector: 'app-admin-create-user',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './admin-create-user.component.html',
  styleUrl: './admin-create-user.component.scss'
})
export class AdminCreateUserComponent implements OnInit {

  roles: string[] = [];
  selectedRoles: string[] = [];
  greska = '';
  loading = false;

  form: AdminCreateKorisnik = {
    userName: '',
    email: '',
    password: '',
    roles: [],
    ime: null,
    prezime: null,
    brojTelefona: null,
    biografija: null
  };

  constructor(
    private adminService: AdminKorisniciService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.adminService.getRoles().subscribe({
      next: (data) => this.roles = data,
      error: () => {}
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
    if (!this.form.userName.trim() || !this.form.email.trim() || !this.form.password.trim()) {
      this.greska = 'Korisnicko ime, email i lozinka su obavezni.';
      return;
    }

    this.greska = '';
    this.loading = true;

    this.adminService.create({ ...this.form, roles: this.selectedRoles }).subscribe({
      next: () => {
        this.router.navigate(['/admin/korisnici']);
      },
      error: (err) => {
        this.greska = err?.error?.message || 'Greska pri kreiranju korisnika.';
        this.loading = false;
      }
    });
  }
}
