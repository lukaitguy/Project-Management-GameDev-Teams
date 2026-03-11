import { Component, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { AuthUser } from '../../../core/models/auth/auth-user.model';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent implements OnInit {

  trenutniKorisnik: AuthUser | null = null;

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.trenutniKorisnik = user;
    });
  }

  odjava(): void {
    this.authService.odjava().subscribe({
      next: () => {
        this.router.navigate(['/prijava']);
      },
      error: (err) => {
        console.error('Greška prilikom odjave:', err);
      }
    })
  }
}
