import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Projekat } from '../models/projekat.model';
import { ProjekatDetails } from '../models/projekat-details.model';
import { CreateProjekat } from '../models/create-projekat.model';

@Injectable({
  providedIn: 'root'
})
export class ProjektiService {

  constructor(private http: HttpClient) { }

  getMojiProjekti(): Observable<Projekat[]> {
    return this.http.get<Projekat[]>('/api/projekti/moji');
  }

  getProjekat(id: string): Observable<ProjekatDetails> {
    return this.http.get<ProjekatDetails>(`/api/projekti/${id}`);
  }

  create(projekat: CreateProjekat): Observable<{ id: string }> {
    return this.http.post<{ id: string }>('/api/projekti', projekat);
  }

  update(id: string, projekat: CreateProjekat): Observable<void> {
    return this.http.put<void>(`/api/projekti/${id}`, projekat);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`/api/projekti/${id}`);
  }
}
