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

  getProjekat(id: string) {
  return this.http.get<ProjekatDetails>(`/api/projekti/${id}`);
  }

  create(projekat: CreateProjekat){
    return this.http.post('/api/projekti', projekat);
  }

  update(id: string, projekat: CreateProjekat){
    return this.http.put(`/api/projekti/${id}`, projekat);
  }
}