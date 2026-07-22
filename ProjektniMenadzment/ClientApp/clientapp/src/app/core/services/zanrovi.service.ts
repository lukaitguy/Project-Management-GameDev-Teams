import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Zanr } from '../models/zanr.model';

@Injectable({
  providedIn: 'root'
})
export class ZanroviService {

  constructor(private http: HttpClient) { }

  getAll(): Observable<Zanr[]> {
    return this.http.get<Zanr[]>('/api/zanrovi');
  }
}
