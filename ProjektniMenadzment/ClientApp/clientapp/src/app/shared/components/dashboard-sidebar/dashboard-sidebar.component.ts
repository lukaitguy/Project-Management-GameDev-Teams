import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { AuthUser } from '../../../core/models/auth/auth-user.model';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-dashboard-sidebar',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './dashboard-sidebar.component.html',
  styleUrl: './dashboard-sidebar.component.scss'
})
export class DashboardSidebarComponent implements OnInit, OnDestroy {

  korisnik: AuthUser | null = null;
  private userSub!: Subscription;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.userSub = this.authService.currentUser$.subscribe(user => {
      this.korisnik = user;
    });
  }

  ngOnDestroy(): void {
    this.userSub.unsubscribe();
  }

  odjava(): void {
    this.authService.odjava();
    this.router.navigate(['/']);
  }
}