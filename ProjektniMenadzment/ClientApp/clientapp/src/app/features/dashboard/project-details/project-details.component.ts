import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { ProjektiService } from '../../../core/services/projekti.service';
import { ProjekatDetalji } from '../../../core/models/projekat-details.model';

@Component({
  selector: 'app-project-details',
  imports: [CommonModule],
  templateUrl: './project-details.component.html',
  styleUrl: './project-details.component.scss'
})
export class ProjectDetailsComponent implements OnInit {

  projekat?: ProjekatDetalji;

  constructor(
    private route: ActivatedRoute,
    private projektiService: ProjektiService
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

  }
}