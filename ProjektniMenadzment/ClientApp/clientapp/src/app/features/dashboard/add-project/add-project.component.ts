import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProjektiService } from '../../../core/services/projekti.service';

@Component({
  selector: 'app-add-project',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './add-project.component.html',
  styleUrl: './add-project.component.scss'
})
export class AddProjectComponent implements OnInit {

  projectForm!: FormGroup;
  loading = false;
  greska = '';
  uspeh = '';

  constructor(
    private fb: FormBuilder,
    private projektiService: ProjektiService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.projectForm = this.fb.group({
      naziv: ['', [Validators.required]],
      opis: [''],
      status: ['Aktivan', [Validators.required]],
      budzet: [null],
      datumPocetka: ['', [Validators.required]],
      rok: [''],
      zanrId: [null],
      verzijaIgre: [''],
      engine: [''],
      platforma: [''],
      fazaRazvoja: ['']
    });
  }

  onSubmit(): void {
    if (this.projectForm.invalid) {
      this.projectForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.greska = '';
    this.uspeh = '';

    this.projektiService.create(this.projectForm.getRawValue()).subscribe({
      next: () => {
        this.uspeh = 'Projekat je uspešno kreiran.';

        setTimeout(() => {
          this.router.navigate(['/moji-projekti']);
        }, 1000);
      },
      error: (err) => {
        console.error(err);
        this.greska = err?.error?.message || 'Došlo je do greške pri kreiranju projekta.';
        this.loading = false;
      },
      complete: () => {
        this.loading = false;
      }
    });
  }
}