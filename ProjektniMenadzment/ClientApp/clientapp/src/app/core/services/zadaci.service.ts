// core/services/zadaci.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MojZadatak } from '../models/moj-zadatak.model';
import { Zadatak } from '../models/zadatak.model';
import { ZadatakDetails } from '../models/zadatak-details.model';
import { CreateZadatak } from '../models/create-zadatak.model';

@Injectable({
  providedIn: 'root'
})
export class ZadaciService {

  constructor(private http: HttpClient) {}

  getMojiZadaci(): Observable<MojZadatak[]> {
    return this.http.get<MojZadatak[]>('/api/zadaci/moji');
  }

  getByProjekatId(projekatId: string): Observable<Zadatak[]> {
    return this.http.get<Zadatak[]>(`/api/projekti/${projekatId}/zadaci`);
  }

  getById(projekatId: string, id: string): Observable<ZadatakDetails> {
    return this.http.get<ZadatakDetails>(`/api/projekti/${projekatId}/zadaci/${id}`);
  }

  create(projekatId: string, zadatak: CreateZadatak): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(`/api/projekti/${projekatId}/zadaci`, zadatak);
  }

  update(projekatId: string, id: string, zadatak: CreateZadatak): Observable<void> {
    return this.http.put<void>(`/api/projekti/${projekatId}/zadaci/${id}`, zadatak);
  }

  updateStatus(projekatId: string, id: string, noviStatus: string): Observable<void> {
    return this.http.patch<void>(
      `/api/projekti/${projekatId}/zadaci/${id}/status`,
      { noviStatus }
    );
  }

  preuzmi(projekatId: string, id: string): Observable<void> {
    return this.http.patch<void>(`/api/projekti/${projekatId}/zadaci/${id}/preuzmi`, {});
  }

  odustani(projekatId: string, id: string): Observable<void> {
    return this.http.patch<void>(`/api/projekti/${projekatId}/zadaci/${id}/odustani`, {});
  }

  delete(projekatId: string, id: string): Observable<void> {
    return this.http.delete<void>(`/api/projekti/${projekatId}/zadaci/${id}`);
  }
}