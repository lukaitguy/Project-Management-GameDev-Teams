import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Profil } from '../models/profil.model';

interface ApiResponse<T> {
  success: boolean;
  message: string | null;
  data: T;
}

@Injectable({ providedIn: 'root' })
export class AccountService {

  constructor(private http: HttpClient) {}

  getProfil(): Observable<ApiResponse<Profil>> {
    return this.http.get<ApiResponse<Profil>>('/api/account');
  }

  updateProfil(dto: {
    ime: string;
    prezime: string;
    brojTelefona?: string | null;
    biografija?: string | null;
  }): Observable<ApiResponse<string>> {
    return this.http.put<ApiResponse<string>>('/api/account', dto);
  }

  promeniKorisnickoIme(dto: {
    novoKorisnickoIme: string;
  }): Observable<ApiResponse<{ token: string; korisnickoIme: string; isAdmin: boolean; isPM: boolean }>> {
    return this.http.put<ApiResponse<{ token: string; korisnickoIme: string; isAdmin: boolean; isPM: boolean }>>(
      '/api/account/korisnicko-ime', dto
    );
  }

  promeniLozinku(dto: {
    trenutnaLozinka: string;
    novaLozinka: string;
  }): Observable<ApiResponse<string>> {
    return this.http.put<ApiResponse<string>>('/api/account/lozinka', dto);
  }
}
