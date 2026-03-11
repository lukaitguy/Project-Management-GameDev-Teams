import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TestService } from './core/services/test.service';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  title = 'clientapp';
  message = '';

  constructor(private authService: AuthService) { }

  ngOnInit() {
    this.authService.ucitajTrenutnogKorisnika().subscribe();
  }


}
