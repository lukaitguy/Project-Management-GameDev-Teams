import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { BuildoviService } from '../../../core/services/buildovi.service';

@Component({
  selector: 'app-add-build',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './add-build.component.html',
  styleUrl: './add-build.component.scss'
})
export class AddBuildComponent implements OnInit {

  projekatId = '';
  buildForm!: FormGroup;

  loading = false;
  greska = '';
  uspeh = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
    private buildoviService: BuildoviService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) return;

    this.projekatId = id;

    this.buildForm = this.fb.group({
      verzija: ['', [Validators.required]],
      nazivBuilda: [''],
      tipBuilda: [''],
      patchNapomene: ['']
    });
  }

  onSubmit(): void {
    if (this.buildForm.invalid) {
      this.buildForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.greska = '';
    this.uspeh = '';

    const requestData = {
      ...this.buildForm.getRawValue(),
      projekatId: this.projekatId
    };

    this.buildoviService.create(requestData).subscribe({
      next: () => {
        this.uspeh = 'Build je uspešno dodat.';

        setTimeout(() => {
          this.router.navigate(['/projekti', this.projekatId, 'buildovi']);
        }, 1000);
      },
      error: (err) => {
        console.error(err);
        this.greska = err?.error?.message || 'Došlo je do greške pri dodavanju builda.';
        this.loading = false;
      },
      complete: () => {
        this.loading = false;
      }
    });
  }
}