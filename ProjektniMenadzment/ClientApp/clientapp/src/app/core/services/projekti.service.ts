import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Projekat } from '../models/projekat.model';
import { ProjekatDetalji } from '../models/projekat-details.model';

@Injectable({
  providedIn: 'root'
})
export class ProjektiService {

  constructor(private http: HttpClient) { }

  getMojiProjekti(): Observable<Projekat[]> {
    return this.http.get<Projekat[]>('/api/projekti/moji');
  }

  getProjekat(id: string) {
  return this.http.get<ProjekatDetalji>(`/api/projekti/${id}`);
}
}