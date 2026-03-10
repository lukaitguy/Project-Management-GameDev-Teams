import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
  AbstractControl,
  ValidationErrors,
  FormGroup
} from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register',
  imports: [RouterLink, ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  porukaGreske = '';
  porukaUspeha = '';
  loading = false;

  registerForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.registerForm = this.fb.group(
    {
      ime: ['', [Validators.required, Validators.minLength(2)]],
      prezime: ['', [Validators.required, Validators.minLength(2)]],
      korisnickoIme: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      brojTelefona: [''],
      lozinka: ['', [Validators.required, Validators.minLength(6)]],
      potvrdaLozinke: ['', [Validators.required]]
    },
    {
      validators: this.lozinkeSePoklapajuValidator
    }
  );
  }


  lozinkeSePoklapajuValidator(group: AbstractControl): ValidationErrors | null {
    const lozinka = group.get('lozinka')?.value;
    const potvrdaLozinke = group.get('potvrdaLozinke')?.value;

    return lozinka === potvrdaLozinke ? null : { lozinkeSeNePoklapaju: true };
  }

  onSubmit() {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.porukaGreske = '';
    this.porukaUspeha = '';

    const { potvrdaLozinke, ...requestData } = this.registerForm.getRawValue();

    this.authService.registracija(requestData as any).subscribe({
      next: (res) => {
        this.porukaUspeha = res.message || 'Registracija uspešna.';

        setTimeout(() => {
          this.router.navigate(['/prijava']);
        }, 1200);
      },
      error: (err) => {
        console.error(err);

        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.porukaGreske = err.error.errors.join(' ');
        } else {
          this.porukaGreske = err?.error?.message || 'Došlo je do greške pri registraciji.';
        }

        this.loading = false;
      },
      complete: () => {
        this.loading = false;
      }
    });
  }
}