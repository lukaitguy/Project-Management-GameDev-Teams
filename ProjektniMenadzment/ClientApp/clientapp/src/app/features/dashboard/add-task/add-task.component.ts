// features/dashboard/add-task/add-task.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ZadaciService } from '../../../core/services/zadaci.service';
import { ClanoviService } from '../../../core/services/clanovi.service';
import { ClanProjekta } from '../../../core/models/clan-projekta.model';
import { CreateZadatak } from '../../../core/models/create-zadatak.model';

@Component({
  selector: 'app-add-task',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './add-task.component.html',
  styleUrl: './add-task.component.scss'
})
export class AddTaskComponent implements OnInit {

  projekatId = '';
  clanovi: ClanProjekta[] = [];
  greska: string | null = null;
  ucitavanjeClanova = true;

  statusi = ['Nije zapocet', 'U toku', 'Pauziran'];
  prioriteti = ['Nizak', 'Srednji', 'Visok'];
  tipoviZadatka = ['Programiranje', 'Dizajn', 'Testiranje', 'Dokumentacija', 'Ostalo'];

  form: CreateZadatak = this.initForm();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private zadaciService: ZadaciService,
    private clanoviService: ClanoviService
  ) {}

  ngOnInit(): void {
    this.projekatId = this.route.snapshot.paramMap.get('id')!;
    this.ucitajClanove();
  }

  ucitajClanove(): void {
    this.clanoviService.getByProjekatId(this.projekatId).subscribe({
      next: (data) => {
        this.clanovi = data;
        this.ucitavanjeClanova = false;
      },
      error: () => {
        this.greska = 'Greška pri učitavanju članova projekta.';
        this.ucitavanjeClanova = false;
      }
    });
  }

  sacuvaj(): void {
    if (!this.form.naslov?.trim()) {
      this.greska = 'Naslov zadatka je obavezan.';
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

    this.greska = null;

    this.zadaciService.create(this.projekatId, this.form).subscribe({
      next: () => {
        this.router.navigate(['/projekti', this.projekatId, 'zadaci']);
      },
      error: (err) => {
        this.greska = err.error?.message ?? 'Greška pri kreiranju zadatka.';
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