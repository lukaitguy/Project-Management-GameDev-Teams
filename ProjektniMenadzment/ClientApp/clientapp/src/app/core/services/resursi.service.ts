import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Resurs } from '../models/resurs.model';
import { CreateResurs } from '../models/create-resurs.model';

@Injectable({
  providedIn: 'root'
})
export class ResursiService {

  constructor(private http: HttpClient) {}

  getByProjekatId(projekatId: string): Observable<Resurs[]> {
    return this.http.get<Resurs[]>(`/api/projekti/${projekatId}/resursi`);
  }

  getById(projekatId: string, id: string): Observable<Resurs> {
    return this.http.get<Resurs>(`/api/projekti/${projekatId}/resursi/${id}`);
  }

  create(projekatId: string, resurs: CreateResurs): Observable<void> {
    return this.http.post<void>(`/api/projekti/${projekatId}/resursi`, resurs);
  }

  update(projekatId: string, id: string, resurs: CreateResurs): Observable<void> {
    return this.http.put<void>(`/api/projekti/${projekatId}/resursi/${id}`, resurs);
  }

  delete(projekatId: string, id: string): Observable<void> {
    return this.http.delete<void>(`/api/projekti/${projekatId}/resursi/${id}`);
  }
}