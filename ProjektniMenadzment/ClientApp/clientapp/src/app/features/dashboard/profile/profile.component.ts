import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../../core/services/account.service';
import { AuthService } from '../../../core/services/auth.service';
import { Profil } from '../../../core/models/profil.model';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit {

  profil?: Profil;
  loading = true;
  greska = '';

  profilForm = { ime: '', prezime: '', brojTelefona: '', biografija: '' };
  profilUspeh = '';
  profilGreska = '';
  profilLoading = false;

  korisnickoImeForm = { novoKorisnickoIme: '' };
  korisnickoImeUspeh = '';
  korisnickoImeGreska = '';
  korisnickoImeLoading = false;

  lozinkaForm = { trenutnaLozinka: '', novaLozinka: '', potvrda: '' };
  lozinkaUspeh = '';
  lozinkaGreska = '';
  lozinkaLoading = false;

  constructor(
    private accountService: AccountService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.accountService.getProfil().subscribe({
      next: (res) => {
        this.profil = res.data;
        this.profilForm = {
          ime: res.data.ime,
          prezime: res.data.prezime,
          brojTelefona: res.data.brojTelefona ?? '',
          biografija: res.data.biografija ?? ''
        };
        this.korisnickoImeForm.novoKorisnickoIme = res.data.korisnickoIme;
        this.loading = false;
      },
      error: () => {
        this.greska = 'Greška pri učitavanju profila.';
        this.loading = false;
      }
    });
  }

  sacuvajProfil(): void {
    if (!this.profilForm.ime.trim() || !this.profilForm.prezime.trim()) {
      this.profilGreska = 'Ime i prezime su obavezni.';
      return;
    }

    this.profilLoading = true;
    this.profilGreska = '';
    this.profilUspeh = '';

    this.accountService.updateProfil({
      ime: this.profilForm.ime,
      prezime: this.profilForm.prezime,
      brojTelefona: this.profilForm.brojTelefona || null,
      biografija: this.profilForm.biografija || null
    }).subscribe({
      next: () => {
        this.profilUspeh = 'Profil je uspešno ažuriran.';
        if (this.profil) {
          this.profil.ime = this.profilForm.ime;
          this.profil.prezime = this.profilForm.prezime;
        }
        this.profilLoading = false;
      },
      error: (err) => {
        this.profilGreska = err.error?.message ?? 'Greška pri ažuriranju profila.';
        this.profilLoading = false;
      }
    });
  }

  sacuvajKorisnickoIme(): void {
    if (!this.korisnickoImeForm.novoKorisnickoIme.trim()) {
      this.korisnickoImeGreska = 'Korisničko ime ne sme biti prazno.';
      return;
    }

    this.korisnickoImeLoading = true;
    this.korisnickoImeGreska = '';
    this.korisnickoImeUspeh = '';

    this.accountService.promeniKorisnickoIme({
      novoKorisnickoIme: this.korisnickoImeForm.novoKorisnickoIme
    }).subscribe({
      next: (res) => {
        localStorage.setItem('auth_token', res.data.token);
        this.authService.refreshUser();
        if (this.profil) this.profil.korisnickoIme = res.data.korisnickoIme;
        this.korisnickoImeUspeh = 'Korisničko ime je uspešno promenjeno.';
        this.korisnickoImeLoading = false;
      },
      error: (err) => {
        this.korisnickoImeGreska = err.error?.message ?? 'Greška pri promeni korisničkog imena.';
        this.korisnickoImeLoading = false;
      }
    });
  }

  sacuvajLozinku(): void {
    if (!this.lozinkaForm.trenutnaLozinka || !this.lozinkaForm.novaLozinka) {
      this.lozinkaGreska = 'Sva polja za lozinku su obavezna.';
      return;
    }
    if (this.lozinkaForm.novaLozinka !== this.lozinkaForm.potvrda) {
      this.lozinkaGreska = 'Nova lozinka i potvrda se ne podudaraju.';
      return;
    }

    this.lozinkaLoading = true;
    this.lozinkaGreska = '';
    this.lozinkaUspeh = '';

    this.accountService.promeniLozinku({
      trenutnaLozinka: this.lozinkaForm.trenutnaLozinka,
      novaLozinka: this.lozinkaForm.novaLozinka
    }).subscribe({
      next: () => {
        this.lozinkaUspeh = 'Lozinka je uspešno promenjena.';
        this.lozinkaForm = { trenutnaLozinka: '', novaLozinka: '', potvrda: '' };
        this.lozinkaLoading = false;
      },
      error: (err) => {
        this.lozinkaGreska = err.error?.message ?? 'Greška pri promeni lozinke.';
        this.lozinkaLoading = false;
      }
    });
  }
}
