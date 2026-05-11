import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Komentar } from '../models/komentar.model';

@Injectable({
  providedIn: 'root'
})
export class KomentariService {

  constructor(private http: HttpClient) {}

  getByZadatakId(projekatId: string, zadatakId: string): Observable<Komentar[]> {
    return this.http.get<Komentar[]>(
      `/api/projekti/${projekatId}/zadaci/${zadatakId}/komentari`
    );
  }

  add(projekatId: string, zadatakId: string, sadrzaj: string): Observable<Komentar> {
    return this.http.post<Komentar>(
      `/api/projekti/${projekatId}/zadaci/${zadatakId}/komentari`,
      { sadrzaj }
    );
  }
}