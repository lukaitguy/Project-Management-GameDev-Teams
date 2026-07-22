import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProjektiService } from '../../../core/services/projekti.service';

@Component({
  selector: 'app-edit-project',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './edit-project.component.html',
  styleUrl: './edit-project.component.scss'
})
export class EditProjectComponent implements OnInit {

  projectForm!: FormGroup;
  projectId = '';
  loading = true;
  saving = false;
  greska = '';
  uspeh = '';

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private projektiService: ProjektiService
  ) { }

  ngOnInit(): void {
    this.projectForm = this.fb.group({
      naziv: ['', [Validators.required]],
      opis: [''],
      status: ['Aktivan', [Validators.required]],
      budzet: [null],
      datumPocetka: ['', [Validators.required]],
      rok: [''],
      verzijaIgre: [''],
      engine: [''],
      platforma: [''],
      fazaRazvoja: ['']
    });

    this.projectId = this.route.snapshot.paramMap.get('id') ?? '';
    if (!this.projectId) {
      this.loading = false;
      this.greska = 'Projekat nije pronadjen.';
      return;
    }

    this.ucitajProjekat();
  }

  onSubmit(): void {
    if (this.projectForm.invalid || !this.projectId) {
      this.projectForm.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.greska = '';
    this.uspeh = '';

    const payload = {
      ...this.projectForm.getRawValue(),
      rok: this.projectForm.value.rok || null
    };

    this.projektiService.update(this.projectId, payload).subscribe({
      next: () => {
        this.uspeh = 'Izmene projekta su uspesno sacuvane.';

        setTimeout(() => {
          this.router.navigate(['/projekti', this.projectId]);
        }, 800);
      },
      error: (err) => {
        console.error(err);
        this.greska = err?.error?.message || 'Doslo je do greske pri izmeni projekta.';
        this.saving = false;
      },
      complete: () => {
        this.saving = false;
      }
    });
  }

  private ucitajProjekat(): void {
    this.loading = true;
    this.greska = '';

    this.projektiService.getProjekat(this.projectId).subscribe({
      next: (projekat) => {
        this.projectForm.patchValue({
          naziv: projekat.naziv,
          opis: projekat.opis ?? '',
          status: projekat.status,
          budzet: projekat.budzet,
          datumPocetka: this.toDateInput(projekat.datumPocetka),
          rok: this.toDateInput(projekat.rok),
          verzijaIgre: projekat.verzijaIgre ?? '',
          engine: projekat.engine ?? '',
          platforma: projekat.platforma ?? '',
          fazaRazvoja: projekat.fazaRazvoja ?? ''
        });
      },
      error: (err) => {
        console.error(err);
        this.greska = err?.error?.message || 'Projekat nije moguce ucitati.';
        this.loading = false;
      },
      complete: () => {
        this.loading = false;
      }
    });
  }

  private toDateInput(value?: string | null): string {
    return value ? value.slice(0, 10) : '';
  }
}
