import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProjekatIzvestaj } from '../models/projekat-izvestaj.model';

@Injectable({
  providedIn: 'root'
})
export class IzvestajiService {

  constructor(private http: HttpClient) {}

  getProjekatIzvestaj(projekatId: string): Observable<ProjekatIzvestaj> {
    return this.http.get<ProjekatIzvestaj>(`/api/izvestaji/projekti/${projekatId}`);
  }
}
