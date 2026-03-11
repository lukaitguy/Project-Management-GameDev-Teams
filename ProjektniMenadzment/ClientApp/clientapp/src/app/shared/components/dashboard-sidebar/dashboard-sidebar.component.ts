import { Component, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { AuthUser } from '../../../core/models/auth/auth-user.model';

@Component({
  selector: 'app-dashboard-sidebar',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './dashboard-sidebar.component.html',
  styleUrl: './dashboard-sidebar.component.scss'
})
export class DashboardSidebarComponent implements OnInit {

  korisnik: AuthUser | null = null;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.korisnik = user;
    });
  }

  odjava() {
    this.authService.odjava().subscribe({
      next: () => {
        this.router.navigate(['/prijava']);
      }
    });
  }
}