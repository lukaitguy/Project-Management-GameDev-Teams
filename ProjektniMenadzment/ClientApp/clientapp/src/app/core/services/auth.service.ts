import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RegisterRequest } from '../models/register-request.model';
import { Observable } from 'rxjs';
import { LoginRequest } from '../models/login-request.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private http: HttpClient) { }

  registracija(data: RegisterRequest): Observable<any> {
    return this.http.post('/api/auth/registracija', data);
  }

  prijava(data: LoginRequest): Observable<any> {
    return this.http.post('/api/auth/prijava', data);
  }
}
