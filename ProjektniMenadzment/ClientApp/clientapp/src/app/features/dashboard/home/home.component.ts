import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Projekat } from '../../../core/models/projekat.model';
import { AuthService } from '../../../core/services/auth.service';
import { ProjektiService } from '../../../core/services/projekti.service';

interface DashboardCard {
  label: string;
  value: string;
  note: string;
}

@Component({
  selector: 'app-home',
  imports: [CommonModule, RouterLink],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {

  projekti: Projekat[] = [];
  istaknutiProjekti: Projekat[] = [];
  kasneciProjekti: Projekat[] = [];
  cards: DashboardCard[] = [];
  loading = false;
  greska = '';

  constructor(
    public authService: AuthService,
    private projektiService: ProjektiService
  ) { }

  get isAdmin(): boolean {
    return this.authService.getCurrentUser()?.isAdmin === true;
  }

  get korisnickoIme(): string {
    return this.authService.getCurrentUser()?.korisnickoIme ?? 'korisnice';
  }

  ngOnInit(): void {
    this.ucitajProjekte();
  }

  private ucitajProjekte(): void {
    this.loading = true;
    this.greska = '';

    this.projektiService.getMojiProjekti().subscribe({
      next: (projekti) => {
        this.projekti = projekti;
        this.pripremiPregled();
      },
      error: (err) => {
        console.error(err);
        this.greska = 'Doslo je do greske pri ucitavanju dashboard podataka.';
        this.loading = false;
      },
      complete: () => {
        this.loading = false;
      }
    });
  }

  private pripremiPregled(): void {
    const danas = this.startOfDay(new Date());
    const zaDveNedelje = new Date(danas);
    zaDveNedelje.setDate(zaDveNedelje.getDate() + 14);

    const aktivniProjekti = this.projekti.filter((projekat) => !this.isClosedStatus(projekat.status));
    const projektiUskoroIsticu = aktivniProjekti.filter((projekat) => {
      const rok = this.parseDate(projekat.rok);
      return !!rok && rok >= danas && rok <= zaDveNedelje;
    });

    this.kasneciProjekti = aktivniProjekti
      .filter((projekat) => {
        const rok = this.parseDate(projekat.rok);
        return !!rok && rok < danas;
      })
      .sort((a, b) => this.compareDates(this.parseDate(a.rok), this.parseDate(b.rok)));

    this.istaknutiProjekti = [...this.projekti]
      .sort((a, b) => {
        const aRok = this.parseDate(a.rok);
        const bRok = this.parseDate(b.rok);

        if (aRok && bRok) {
          return aRok.getTime() - bRok.getTime();
        }

        if (aRok) {
          return -1;
        }

        if (bRok) {
          return 1;
        }

        return this.compareDates(this.parseDate(b.datumPocetka), this.parseDate(a.datumPocetka));
      })
      .slice(0, 4);

    const poslednjiBuild = [...this.projekti]
      .map((projekat) => this.parseDate(projekat.datumPoslednjegBuilda))
      .filter((datum): datum is Date => datum !== null)
      .sort((a, b) => b.getTime() - a.getTime())[0];

    this.cards = [
      {
        label: this.isAdmin ? 'Ukupno projekata' : 'Moji projekti',
        value: `${this.projekti.length}`,
        note: this.isAdmin ? 'Pregled svih projekata u sistemu.' : 'Projekti na kojima trenutno ucestvujete.'
      },
      {
        label: 'Aktivni projekti',
        value: `${aktivniProjekti.length}`,
        note: 'Status nije zavrsen ili otkazan.'
      },
      {
        label: 'Rok u 14 dana',
        value: `${projektiUskoroIsticu.length}`,
        note: 'Projekti kojima uskoro istice planirani rok.'
      },
      {
        label: 'Poslednji build',
        value: poslednjiBuild ? this.formatDateTime(poslednjiBuild) : 'Nema',
        note: 'Najnoviji evidentirani build na dostupnim projektima.'
      }
    ];
  }

  formatDate(value?: string | null): string {
    const parsed = this.parseDate(value);
    if (!parsed) {
      return 'Nije definisan';
    }

    return parsed.toLocaleDateString('sr-RS');
  }

  private parseDate(value?: string | null): Date | null {
    if (!value) {
      return null;
    }

    const parsed = new Date(value);
    return Number.isNaN(parsed.getTime()) ? null : parsed;
  }

  private startOfDay(value: Date): Date {
    return new Date(value.getFullYear(), value.getMonth(), value.getDate());
  }

  private formatDateTime(value: Date): string {
    return value.toLocaleString('sr-RS', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  private isClosedStatus(status: string): boolean {
    const normalized = status.trim().toLowerCase();
    return normalized === 'zavrsen' || normalized === 'zavrseno' || normalized === 'otkazan' || normalized === 'zatvoren';
  }

  private compareDates(left: Date | null, right: Date | null): number {
    const leftValue = left?.getTime() ?? Number.MAX_SAFE_INTEGER;
    const rightValue = right?.getTime() ?? Number.MAX_SAFE_INTEGER;
    return leftValue - rightValue;
  }
}
