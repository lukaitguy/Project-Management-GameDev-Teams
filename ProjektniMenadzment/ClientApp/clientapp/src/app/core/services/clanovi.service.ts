// core/services/clanovi.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ClanProjekta } from '../models/clan-projekta.model';
import { AddClan } from '../models/add-clan.model';

@Injectable({
  providedIn: 'root'
})
export class ClanoviService {

  constructor(private http: HttpClient) {}

  getByProjekatId(projekatId: string): Observable<ClanProjekta[]> {
    return this.http.get<ClanProjekta[]>(`/api/projekti/${projekatId}/clanovi`);
  }

  add(projekatId: string, clan: AddClan): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(
      `/api/projekti/${projekatId}/clanovi`, clan
    );
  }

  remove(projekatId: string, korisnikId: string): Observable<void> {
    return this.http.delete<void>(
      `/api/projekti/${projekatId}/clanovi/${korisnikId}`
    );
  }
}