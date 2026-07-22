import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { ProjektiService } from '../../../core/services/projekti.service';
import { ZanroviService } from '../../../core/services/zanrovi.service';
import { Zanr } from '../../../core/models/zanr.model';

function rokAfterDatumPocetkaValidator(control: AbstractControl): ValidationErrors | null {
  const datumPocetka = control.get('datumPocetka')?.value;
  const rok = control.get('rok')?.value;

  if (!datumPocetka || !rok) {
    return null;
  }

  return new Date(rok) < new Date(datumPocetka) ? { rokPreDatumaPocetka: true } : null;
}

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

  zanrovi: Zanr[] = [];
  selectedZanrIds: string[] = [];

  constructor(
    private fb: FormBuilder,
    private projektiService: ProjektiService,
    private zanroviService: ZanroviService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.projectForm = this.fb.group({
      naziv: ['', [Validators.required, Validators.maxLength(50)]],
      opis: ['', [Validators.maxLength(150)]],
      status: ['Aktivan', [Validators.required, Validators.maxLength(50)]],
      budzet: [null, [Validators.min(0)]],
      datumPocetka: ['', [Validators.required]],
      rok: [''],
      verzijaIgre: ['', [Validators.maxLength(20)]],
      engine: ['', [Validators.maxLength(50)]],
      platforma: ['', [Validators.maxLength(100)]],
      fazaRazvoja: ['', [Validators.maxLength(30)]]
    }, { validators: rokAfterDatumPocetkaValidator });

    this.zanroviService.getAll().subscribe({
      next: (zanrovi) => this.zanrovi = zanrovi,
      error: (err) => console.error(err)
    });
  }

  fieldInvalid(controlName: string): boolean {
    const control = this.projectForm.get(controlName);
    return !!control && control.invalid && (control.touched || control.dirty);
  }

  get rokPreDatumaPocetka(): boolean {
    const rok = this.projectForm.get('rok');
    return this.projectForm.hasError('rokPreDatumaPocetka') && (!!rok?.touched || !!rok?.dirty);
  }

  toggleZanr(zanrId: string, checked: boolean): void {
    if (checked) {
      if (!this.selectedZanrIds.includes(zanrId)) {
        this.selectedZanrIds.push(zanrId);
      }
    } else {
      this.selectedZanrIds = this.selectedZanrIds.filter(id => id !== zanrId);
    }
  }

  onSubmit(): void {
    if (this.projectForm.invalid) {
      this.projectForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.greska = '';
    this.uspeh = '';

    const payload = {
      ...this.projectForm.getRawValue(),
      rok: this.projectForm.value.rok || null,
      selectedZanrIds: this.selectedZanrIds
    };

    this.projektiService.create(payload).subscribe({
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