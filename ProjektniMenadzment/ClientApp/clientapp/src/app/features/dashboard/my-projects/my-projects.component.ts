import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ProjektiService } from '../../../core/services/projekti.service';
import { Projekat } from '../../../core/models/projekat.model';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-my-projects',
  imports: [CommonModule, RouterLink],
  templateUrl: './my-projects.component.html',
  styleUrl: './my-projects.component.scss'
})
export class MyProjectsComponent implements OnInit {

  projekti: Projekat[] = [];
  loading = false;
  greska = '';
  uspeh = '';

  constructor(
    private projektiService: ProjektiService,
    public authService: AuthService
  ) { }

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
    this.ucitajProjekte();
  }

  ucitajProjekte(): void {
    this.loading = true;
    this.greska = '';

    this.projektiService.getMojiProjekti().subscribe({
      next: (res) => {
        this.projekti = res;
      },
      error: (err) => {
        console.error(err);
        this.greska = err?.error?.message || 'Doslo je do greske pri ucitavanju projekata.';
        this.loading = false;
      },
      complete: () => {
        this.loading = false;
      }
    });
  }

  obrisiProjekat(event: MouseEvent, projekatId: string): void {
    event.preventDefault();
    event.stopPropagation();

    if (!window.confirm('Da li zelite da obrisete projekat?')) {
      return;
    }

    this.greska = '';
    this.uspeh = '';

    this.projektiService.delete(projekatId).subscribe({
      next: () => {
        this.projekti = this.projekti.filter((projekat) => projekat.id !== projekatId);
        this.uspeh = 'Projekat je uspesno obrisan.';
      },
      error: (err) => {
        console.error(err);
        this.greska = err?.error?.message || 'Doslo je do greske pri brisanju projekta.';
      }
    });
  }
}
