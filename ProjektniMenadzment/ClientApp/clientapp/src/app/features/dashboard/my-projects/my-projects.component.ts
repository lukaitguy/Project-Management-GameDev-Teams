import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProjektiService } from '../../../core/services/projekti.service';
import { Projekat } from '../../../core/models/projekat.model';
import { RouterLink } from '@angular/router';

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

  constructor(private projektiService: ProjektiService) { }

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
        this.greska = 'Došlo je do greške pri učitavanju projekata.';
        this.loading = false;
      },
      complete: () => {
        this.loading = false;
      }
    });
  }
}