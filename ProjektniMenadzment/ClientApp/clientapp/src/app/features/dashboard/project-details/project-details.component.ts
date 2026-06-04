// features/dashboard/project-details/project-details.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ProjektiService } from '../../../core/services/projekti.service';
import { ProjekatDetails } from '../../../core/models/projekat-details.model';
import { BuildoviService } from '../../../core/services/buildovi.service';
import { Build } from '../../../core/models/build.model';
import { ProjectMembersComponent } from '../project-members/project-members.component';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-project-details',
  imports: [CommonModule, RouterLink, ProjectMembersComponent],
  templateUrl: './project-details.component.html',
  styleUrl: './project-details.component.scss'
})
export class ProjectDetailsComponent implements OnInit {

  projekat?: ProjekatDetails;
  buildovi: Build[] = [];
  poslednjiBuild?: Build;
  loadingBuildovi = false;
  greskaBuildovi = '';
  isAdmin = false;
  isPM = false;

  get canManage(): boolean {
    return this.isAdmin || this.isPM;
  }

  get statusClass(): string {
    switch (this.projekat?.status?.toLowerCase()) {
      case 'aktivan':      return 'status-aktivan';
      case 'završen':
      case 'zavrsen':      return 'status-zavrsen';
      case 'pauziran':     return 'status-pauziran';
      case 'otkazan':      return 'status-otkazan';
      default:             return 'status-default';
    }
  }

  constructor(
    private route: ActivatedRoute,
    private projektiService: ProjektiService,
    private buildoviService: BuildoviService,
    private authService: AuthService 
  ) {}

  ngOnInit(): void {
    const user = this.authService.getCurrentUser();
    this.isAdmin = user?.isAdmin ?? false;
    this.isPM = user?.isPM ?? false;
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;

    this.projektiService.getProjekat(id).subscribe({
      next: (res) => this.projekat = res,
      error: (err) => console.error(err)
    });

    this.ucitajBuildove(id);
  }

  ucitajBuildove(projekatId: string): void {
    this.loadingBuildovi = true;
    this.greskaBuildovi = '';

    this.buildoviService.getByProjekatId(projekatId).subscribe({
      next: (res) => {
        this.buildovi = res;
        this.poslednjiBuild = res.length > 0 ? res[0] : undefined;
      },
      error: (err) => {
        console.error(err);
        this.greskaBuildovi = 'Došlo je do greške pri učitavanju buildova.';
        this.loadingBuildovi = false;
      },
      complete: () => this.loadingBuildovi = false
    });
  }

}