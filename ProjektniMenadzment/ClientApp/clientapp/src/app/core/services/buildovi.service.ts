import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Build } from '../models/build.model';
import { CreateBuild } from '../models/create-build.model';

@Injectable({
  providedIn: 'root'
})
export class BuildoviService {

  constructor(private http: HttpClient) { }

  getByProjekatId(projekatId: string): Observable<Build[]> {
    return this.http.get<Build[]>(`/api/buildovi/projekat/${projekatId}`);
  }

  create(build: CreateBuild) {
    return this.http.post('/api/buildovi', build);
  }
}