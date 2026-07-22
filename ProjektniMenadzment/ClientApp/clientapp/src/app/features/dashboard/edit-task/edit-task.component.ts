// features/dashboard/edit-task/edit-task.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ZadaciService } from '../../../core/services/zadaci.service';
import { ClanoviService } from '../../../core/services/clanovi.service';
import { ProjektiService } from '../../../core/services/projekti.service';
import { ClanProjekta } from '../../../core/models/clan-projekta.model';
import { CreateZadatak } from '../../../core/models/create-zadatak.model';
import { ProjekatDetails } from '../../../core/models/projekat-details.model';

@Component({
  selector: 'app-edit-task',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './edit-task.component.html',
  styleUrl: './edit-task.component.scss'
})
export class EditTaskComponent implements OnInit {

  projekatId = '';
  zadatakId = '';
  clanovi: ClanProjekta[] = [];
  projekat: ProjekatDetails | null = null;
  greska: string | null = null;
  uspeh: string | null = null;
  ucitavanje = true;
  cuvanje = false;

  statusi = ['Nije zapocet', 'U toku', 'Pauziran', 'Otkazan', 'Zavrsen'];
  prioriteti = ['Nizak', 'Srednji', 'Visok'];
  tipoviZadatka = ['Programiranje', 'Dizajn', 'Testiranje', 'Dokumentacija', 'Ostalo'];

  form: CreateZadatak = this.initForm();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private zadaciService: ZadaciService,
    private clanoviService: ClanoviService,
    private projektiService: ProjektiService
  ) {}

  ngOnInit(): void {
    this.projekatId = this.route.snapshot.paramMap.get('id')!;
    this.zadatakId = this.route.snapshot.paramMap.get('zadatakId')!;
    this.ucitajClanove();
    this.projektiService.getProjekat(this.projekatId).subscribe({
      next: (data) => this.projekat = data,
      error: () => { }
    });
    this.ucitajZadatak();
  }

  get minRok(): string | null {
    return this.projekat?.datumPocetka ? this.projekat.datumPocetka.substring(0, 10) : null;
  }

  get maxRok(): string | null {
    return this.projekat?.rok ?? null;
  }

  ucitajClanove(): void {
    this.clanoviService.getByProjekatId(this.projekatId).subscribe({
      next: (data) => this.clanovi = data,
      error: () => { }
    });
  }

  ucitajZadatak(): void {
    this.zadaciService.getById(this.projekatId, this.zadatakId).subscribe({
      next: (data) => {
        this.form = {
          naslov: data.naslov,
          opis: data.opis,
          status: data.status,
          prioritet: data.prioritet,
          tipZadatka: data.tipZadatka,
          rok: data.rok ? data.rok.substring(0, 10) : null,
          dodeljenKorisnikuId: data.dodeljenKorisnikuId
        };
        this.ucitavanje = false;
      },
      error: () => {
        this.greska = 'Greška pri učitavanju zadatka.';
        this.ucitavanje = false;
      }
    });
  }

  sacuvaj(): void {
    if (!this.form.naslov?.trim()) {
      this.greska = 'Naslov zadatka je obavezan.';
      return;
    }
    if (this.form.naslov.trim().length > 50) {
      this.greska = 'Naslov zadatka ne sme imati više od 50 karaktera.';
      return;
    }
    if (!this.form.status) {
      this.greska = 'Status je obavezan.';
      return;
    }
    if (!this.form.prioritet) {
      this.greska = 'Prioritet je obavezan.';
      return;
    }
    if (this.form.opis && this.form.opis.length > 150) {
      this.greska = 'Opis ne sme imati više od 150 karaktera.';
      return;
    }
    if (this.form.tipZadatka && this.form.tipZadatka.length > 30) {
      this.greska = 'Tip zadatka ne sme imati više od 30 karaktera.';
      return;
    }
    if (this.form.rok && this.projekat) {
      const rok = new Date(this.form.rok);
      if (rok < new Date(this.minRok!)) {
        this.greska = 'Rok zadatka ne može biti pre datuma početka projekta.';
        return;
      }
      if (this.projekat.rok && rok > new Date(this.projekat.rok)) {
        this.greska = 'Rok zadatka ne može biti posle roka projekta.';
        return;
      }
    }

    this.greska = null;
    this.uspeh = null;
    this.cuvanje = true;

    const payload: CreateZadatak = {
      ...this.form,
      rok: this.form.rok || null
    };

    this.zadaciService.update(this.projekatId, this.zadatakId, payload).subscribe({
      next: () => {
        this.uspeh = 'Izmene zadatka su uspešno sačuvane.';
        this.cuvanje = false;
        setTimeout(() => {
          this.router.navigate(['/projekti', this.projekatId, 'zadaci']);
        }, 800);
      },
      error: (err) => {
        this.greska = err.error?.message ?? 'Greška pri izmeni zadatka.';
        this.cuvanje = false;
      }
    });
  }

  private initForm(): CreateZadatak {
    return {
      naslov: '',
      opis: null,
      status: 'Nije zapocet',
      prioritet: 'Srednji',
      tipZadatka: null,
      rok: null,
      dodeljenKorisnikuId: null
    };
  }
}
