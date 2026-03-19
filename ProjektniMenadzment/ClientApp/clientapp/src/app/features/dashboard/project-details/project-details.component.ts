import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ProjektiService } from '../../../core/services/projekti.service';
import { ProjekatDetails } from '../../../core/models/projekat-details.model';
import { BuildoviService } from '../../../core/services/buildovi.service';
import { Build } from '../../../core/models/build.model';

@Component({
  selector: 'app-project-details',
  imports: [CommonModule, RouterLink],
  templateUrl: './project-details.component.html',
  styleUrl: './project-details.component.scss'
})
export class ProjectDetailsComponent implements OnInit {

  projekat?: ProjekatDetails;
  buildovi: Build[] = [];
  poslednjiBuild?: Build;
  loadingBuildovi = false;
  greskaBuildovi = '';

  constructor(
    private route: ActivatedRoute,
    private projektiService: ProjektiService,
    private buildoviService: BuildoviService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) return;

    this.projektiService.getProjekat(id).subscribe({
      next: (res) => {
        this.projekat = res;
      },
      error: (err) => {
        console.error(err);
      }
    });

    this.ucitajBuildove(id);
  }

  ucitajBuildove(projekatId: string): void {
    this.loadingBuildovi = true;
    this.greskaBuildovi = '';

    this.buildoviService.getByProjekatId(projekatId).subscribe({
      next: (res) => {
        this.buildovi = res;
        this.poslednjiBuild = this.buildovi.length > 0 ? this.buildovi[0] : undefined;
      },
      error: (err) => {
        console.error(err);
        this.greskaBuildovi = 'Došlo je do greške pri učitavanju buildova.';
        this.loadingBuildovi = false;
      },
      complete: () => {
        this.loadingBuildovi = false;
      }
    });
  }
}