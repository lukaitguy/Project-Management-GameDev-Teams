import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ResursiService } from '../../../core/services/resursi.service';
import { ClanoviService } from '../../../core/services/clanovi.service';
import { AuthService } from '../../../core/services/auth.service';
import { Resurs } from '../../../core/models/resurs.model';
import { CreateResurs } from '../../../core/models/create-resurs.model';
import { ClanProjekta } from '../../../core/models/clan-projekta.model';

@Component({
  selector: 'app-project-resources',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './project-resources.component.html',
  styleUrl: './project-resources.component.scss'
})
export class ProjectResourcesComponent implements OnInit {

  projekatId!: string;
  resursi: Resurs[] = [];
  clanovi: ClanProjekta[] = [];
  isAdmin = false;
  ucitavanje = true;
  greska: string | null = null;

  showForm = false;
  editingId: string | null = null;

  tipoviResursa = ['Hardware', 'Software', 'Licenca', 'Alat', 'Ostalo'];

  form: CreateResurs = this.initForm();

  constructor(
    private route: ActivatedRoute,
    private resursiService: ResursiService,
    private clanoviService: ClanoviService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.projekatId = this.route.snapshot.paramMap.get('id')!;
    this.isAdmin = this.authService.getCurrentUser()?.isAdmin ?? false;
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
      error: () => { this.greska = 'Greška pri učitavanju članova projekta.'; }
    });
  }

  sacuvajResurs(): void {
    if (!this.form.naziv || !this.form.tip) {
      this.greska = 'Naziv i tip su obavezni.';
      return;
    }

    const request = this.editingId
      ? this.resursiService.update(this.projekatId, this.editingId, this.form)
      : this.resursiService.create(this.projekatId, this.form);

    request.subscribe({
      next: () => {
        this.resetForm();
        this.ucitajResurse();
      },
      error: (err) => {
        this.greska = err.error?.message ?? 'Greška pri čuvanju resursa.';
      }
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
    this.showForm = true;
    this.greska = null;
  }

  obrisiResurs(id: string): void {
    if (!confirm('Obrisati resurs?')) return;
    this.resursiService.delete(this.projekatId, id).subscribe({
      next: () => this.resursi = this.resursi.filter(r => r.id !== id),
      error: () => this.greska = 'Greška pri brisanju resursa.'
    });
  }

  toggleForm(): void {
    if (this.showForm) {
      this.resetForm();
    } else {
      this.showForm = true;
      this.greska = null;
    }
  }

  private resetForm(): void {
    this.showForm = false;
    this.editingId = null;
    this.form = this.initForm();
    this.greska = null;
  }

  private initForm(): CreateResurs {
    return {
      naziv: '',
      tip: '',
      opis: null,
      cena: null,
      dodeljenKorisniku: null
    };
  }
}
