import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  imports: [RouterLink, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  porukaGreske = '';
  loading = false;

  loginForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      emailIliKorisnickoIme: ['', [Validators.required]],
      lozinka: ['', [Validators.required, Validators.minLength(6)]],
      zapamtiMe: [false]
    });
  }

onSubmit() {
  if (this.loading) {
    return;
  }

  if (this.loginForm.invalid) {
    this.loginForm.markAllAsTouched();
    return;
  }

  this.loading = true;
  this.porukaGreske = '';

  this.authService.prijava(this.loginForm.getRawValue()).subscribe({
    next: () => {
      this.router.navigate(['/dashboard']);
    },
    error: (err) => {
      console.error(err);
      this.porukaGreske = err?.error?.message || 'Došlo je do greške pri prijavi.';
      this.loading = false;
    },
    complete: () => {
      this.loading = false;
    }
  });
}
}
