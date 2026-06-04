import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ClanoviService } from '../../../core/services/clanovi.service';
import { AuthService } from '../../../core/services/auth.service';
import { ClanProjekta } from '../../../core/models/clan-projekta.model';

@Component({
  selector: 'app-project-members-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './project-members-page.component.html',
  styleUrl: './project-members-page.component.scss'
})
export class ProjectMembersPageComponent implements OnInit {

  projekatId = '';
  clanovi: ClanProjekta[] = [];

  ucitavanje = true;
  greska = '';

  canManage = false;
  showForm = false;
  dodavanjeLoading = false;
  dodavanjeGreska = '';
  dodavanjeUspeh = '';

  uklananjeId: string | null = null;

  noviEmail = '';
  novaUloga = '';

  uloge = ['Developer', 'Designer', 'Tester', 'Projektni Menadzer', 'Ostalo'];

  private readonly AVATAR_PALETTE = [
    '#6366f1', '#8b5cf6', '#ec4899',
    '#f59e0b', '#10b981', '#3b82f6', '#ef4444'
  ];

  private readonly ULOGA_COLORS: Record<string, { bg: string; text: string }> = {
    'Developer':         { bg: '#eff6ff', text: '#1d4ed8' },
    'Designer':          { bg: '#f5f3ff', text: '#7c3aed' },
    'Tester':            { bg: '#f0fdf4', text: '#15803d' },
    'Projektni Menadzer':{ bg: '#fef9c3', text: '#a16207' },
    'Ostalo':            { bg: '#f1f5f9', text: '#475569' },
  };

  constructor(
    private route: ActivatedRoute,
    private clanoviService: ClanoviService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.projekatId = this.route.snapshot.paramMap.get('id')!;
    const user = this.authService.getCurrentUser();
    this.canManage = (user?.isAdmin || user?.isPM) ?? false;
    this.ucitajClanove();
  }

  ucitajClanove(): void {
    this.ucitavanje = true;
    this.greska = '';
    this.clanoviService.getByProjekatId(this.projekatId).subscribe({
      next: (data) => { this.clanovi = data; this.ucitavanje = false; },
      error: () => { this.greska = 'Greška pri učitavanju članova.'; this.ucitavanje = false; }
    });
  }

  toggleForm(): void {
    this.showForm = !this.showForm;
    if (!this.showForm) { this.resetForm(); }
  }

  dodajClana(): void {
    if (!this.noviEmail.trim()) {
      this.dodavanjeGreska = 'Email je obavezan.';
      return;
    }

    this.dodavanjeLoading = true;
    this.dodavanjeGreska = '';
    this.dodavanjeUspeh = '';

    this.clanoviService.add(this.projekatId, {
      email: this.noviEmail.trim(),
      uloga: this.novaUloga || null
    }).subscribe({
      next: () => {
        this.dodavanjeUspeh = 'Član je uspešno dodat.';
        this.dodavanjeLoading = false;
        this.noviEmail = '';
        this.novaUloga = '';
        this.ucitajClanove();
      },
      error: (err) => {
        this.dodavanjeGreska = err.error?.message ?? 'Greška pri dodavanju člana.';
        this.dodavanjeLoading = false;
      }
    });
  }

  ukloniClana(korisnikId: string): void {
    if (!confirm('Ukloniti člana sa projekta?')) return;
    this.uklananjeId = korisnikId;

    this.clanoviService.remove(this.projekatId, korisnikId).subscribe({
      next: () => {
        this.clanovi = this.clanovi.filter(c => c.korisnikId !== korisnikId);
        this.uklananjeId = null;
      },
      error: () => {
        this.greska = 'Greška pri uklanjanju člana.';
        this.uklananjeId = null;
      }
    });
  }

  initials(clan: ClanProjekta): string {
    return ((clan.ime?.[0] ?? '') + (clan.prezime?.[0] ?? '')).toUpperCase();
  }

  avatarColor(korisnikId: string): string {
    const idx = korisnikId.charCodeAt(0) % this.AVATAR_PALETTE.length;
    return this.AVATAR_PALETTE[idx];
  }

  ulogaStyle(uloga: string | null | undefined): { background: string; color: string } {
    const def = { background: '#f1f5f9', color: '#475569' };
    if (!uloga) return def;
    const s = this.ULOGA_COLORS[uloga];
    return s ? { background: s.bg, color: s.text } : def;
  }

  private resetForm(): void {
    this.noviEmail = '';
    this.novaUloga = '';
    this.dodavanjeGreska = '';
    this.dodavanjeUspeh = '';
  }
}
