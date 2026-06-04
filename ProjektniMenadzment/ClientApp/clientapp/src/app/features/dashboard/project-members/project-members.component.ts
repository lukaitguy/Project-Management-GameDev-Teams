import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ClanoviService } from '../../../core/services/clanovi.service';
import { ClanProjekta } from '../../../core/models/clan-projekta.model';

@Component({
  selector: 'app-project-members',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './project-members.component.html',
  styleUrl: './project-members.component.scss'
})
export class ProjectMembersComponent implements OnInit {

  @Input() projekatId!: string;

  clanovi: ClanProjekta[] = [];
  ucitavanje = true;
  greska = '';

  private readonly PREVIEW_COUNT = 6;
  private readonly AVATAR_PALETTE = [
    '#6366f1', '#8b5cf6', '#ec4899',
    '#f59e0b', '#10b981', '#3b82f6', '#ef4444'
  ];

  constructor(private clanoviService: ClanoviService) {}

  ngOnInit(): void {
    this.clanoviService.getByProjekatId(this.projekatId).subscribe({
      next: (data) => { this.clanovi = data; this.ucitavanje = false; },
      error: () => { this.greska = 'Greška pri učitavanju.'; this.ucitavanje = false; }
    });
  }

  get preview(): ClanProjekta[] {
    return this.clanovi.slice(0, this.PREVIEW_COUNT);
  }

  get remaining(): number {
    return Math.max(0, this.clanovi.length - this.PREVIEW_COUNT);
  }

  initials(clan: ClanProjekta): string {
    return ((clan.ime?.[0] ?? '') + (clan.prezime?.[0] ?? '')).toUpperCase();
  }

  avatarColor(korisnikId: string): string {
    const idx = korisnikId.charCodeAt(0) % this.AVATAR_PALETTE.length;
    return this.AVATAR_PALETTE[idx];
  }
}
