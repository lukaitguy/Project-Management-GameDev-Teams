import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AdminCreateKorisnik, AdminKorisnik, AdminUpdateKorisnik } from '../models/admin-korisnik.model';

@Injectable({
  providedIn: 'root'
})
export class AdminKorisniciService {

  constructor(private http: HttpClient) {}

  getAll(search?: string, role?: string): Observable<AdminKorisnik[]> {
    let params = new HttpParams();
    if (search) params = params.set('search', search);
    if (role) params = params.set('role', role);
    return this.http.get<AdminKorisnik[]>('/api/admin/korisnici', { params });
  }

  getById(identityUserId: string): Observable<AdminKorisnik> {
    return this.http.get<AdminKorisnik>(`/api/admin/korisnici/${identityUserId}`);
  }

  getRoles(): Observable<string[]> {
    return this.http.get<string[]>('/api/admin/korisnici/roles');
  }

  create(dto: AdminCreateKorisnik): Observable<void> {
    return this.http.post<void>('/api/admin/korisnici', dto);
  }

  update(identityUserId: string, dto: AdminUpdateKorisnik): Observable<void> {
    return this.http.put<void>(`/api/admin/korisnici/${identityUserId}`, dto);
  }

  changePassword(identityUserId: string, novaLozinka: string): Observable<void> {
    return this.http.put<void>(`/api/admin/korisnici/${identityUserId}/lozinka`, { novaLozinka });
  }

  delete(identityUserId: string): Observable<void> {
    return this.http.delete<void>(`/api/admin/korisnici/${identityUserId}`);
  }
}
