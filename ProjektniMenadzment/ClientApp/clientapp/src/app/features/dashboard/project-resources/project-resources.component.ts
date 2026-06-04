import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ResursiService } from '../../../core/services/resursi.service';
import { ClanoviService } from '../../../core/services/clanovi.service';
import { AuthService } from '../../../core/services/auth.service';
import { Resurs } from '../../../core/models/resurs.model';
import { CreateResurs } from '../../../core/models/create-resurs.model';
import { ClanProjekta } from '../../../core/models/clan-projekta.model';

@Component({
  selector: 'app-project-resources',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './project-resources.component.html',
  styleUrl: './project-resources.component.scss'
})
export class ProjectResourcesComponent implements OnInit {

  projekatId!: string;
  resursi: Resurs[] = [];
  clanovi: ClanProjekta[] = [];
  ucitavanje = true;
  greska: string | null = null;

  showEditForm = false;
  editingId: string | null = null;

  tipoviResursa = ['Software', 'Hardware', 'Drugo'];

  form: CreateResurs = this.initForm();

  constructor(
    private route: ActivatedRoute,
    private resursiService: ResursiService,
    private clanoviService: ClanoviService,
    private authService: AuthService
  ) {}

  get isAdmin(): boolean {
    return this.authService.getCurrentUser()?.isAdmin === true;
  }

  get isPM(): boolean {
    return this.authService.getCurrentUser()?.isPM === true;
  }

  get canManage(): boolean {
    return this.isAdmin || this.isPM;
  }

  ngOnInit(): void {
    this.projekatId = this.route.snapshot.paramMap.get('id')!;
    this.ucitajResurse();
    this.ucitajClanove();
  }

  ucitajResurse(): void {
    this.resursiService.getByProjekatId(this.projekatId).subscribe({
      next: (data) => {
        this.resursi = data;
        this.ucitavanje = false;
      },
      error: () => {
        this.greska = 'Greška pri učitavanju resursa.';
        this.ucitavanje = false;
      }
    });
  }

  ucitajClanove(): void {
    this.clanoviService.getByProjekatId(this.projekatId).subscribe({
      next: (data) => this.clanovi = data,
      error: () => {}
    });
  }

  urediResurs(resurs: Resurs): void {
    this.editingId = resurs.id;
    this.form = {
      naziv: resurs.naziv,
      tip: resurs.tip,
      opis: resurs.opis,
      cena: resurs.cena,
      dodeljenKorisniku: resurs.dodeljenKorisniku
    };
    this.showEditForm = true;
    this.greska = null;
  }

  sacuvajIzmenu(): void {
    if (!this.form.naziv || !this.form.tip) {
      this.greska = 'Naziv i tip su obavezni.';
      return;
    }
    this.resursiService.update(this.projekatId, this.editingId!, this.form).subscribe({
      next: () => {
        this.resetForm();
        this.ucitajResurse();
      },
      error: (err) => {
        this.greska = err.error?.message ?? 'Greška pri čuvanju resursa.';
      }
    });
  }

  otkaziIzmenu(): void {
    this.resetForm();
  }

  obrisiResurs(id: string): void {
    if (!confirm('Obrisati resurs?')) return;
    this.resursiService.delete(this.projekatId, id).subscribe({
      next: () => this.resursi = this.resursi.filter(r => r.id !== id),
      error: () => this.greska = 'Greška pri brisanju resursa.'
    });
  }

  private resetForm(): void {
    this.showEditForm = false;
    this.editingId = null;
    this.form = this.initForm();
    this.greska = null;
  }

  private initForm(): CreateResurs {
    return { naziv: '', tip: '', opis: null, cena: null, dodeljenKorisniku: null };
  }
}
