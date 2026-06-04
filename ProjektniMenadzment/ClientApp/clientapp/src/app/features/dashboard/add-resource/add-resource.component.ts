import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CreateResurs } from '../../../core/models/create-resurs.model';
import { ClanProjekta } from '../../../core/models/clan-projekta.model';
import { ResursiService } from '../../../core/services/resursi.service';
import { ClanoviService } from '../../../core/services/clanovi.service';

@Component({
  selector: 'app-add-resource',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './add-resource.component.html',
  styleUrl: './add-resource.component.scss'
})
export class AddResourceComponent implements OnInit {

  projekatId = '';
  clanovi: ClanProjekta[] = [];
  greska: string | null = null;
  ucitavanjeClanova = true;

  tipoviResursa = ['Software', 'Hardware', 'Drugo'];

  form: CreateResurs = {
    naziv: '',
    tip: '',
    opis: null,
    cena: null,
    dodeljenKorisniku: null
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private resursiService: ResursiService,
    private clanoviService: ClanoviService
  ) {}

  ngOnInit(): void {
    this.projekatId = this.route.snapshot.paramMap.get('id')!;
    this.clanoviService.getByProjekatId(this.projekatId).subscribe({
      next: (data) => {
        this.clanovi = data;
        this.ucitavanjeClanova = false;
      },
      error: () => {
        this.ucitavanjeClanova = false;
      }
    });
  }

  sacuvaj(): void {
    if (!this.form.naziv?.trim()) {
      this.greska = 'Naziv resursa je obavezan.';
      return;
    }
    if (!this.form.tip) {
      this.greska = 'Tip resursa je obavezan.';
      return;
    }

    this.greska = null;

    this.resursiService.create(this.projekatId, this.form).subscribe({
      next: () => {
        this.router.navigate(['/projekti', this.projekatId, 'resursi']);
      },
      error: (err) => {
        this.greska = err.error?.message ?? 'Greška pri kreiranju resursa.';
      }
    });
  }
}
