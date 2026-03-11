import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RegisterRequest } from '../models/register-request.model';
import { BehaviorSubject, catchError, Observable, of, tap, switchMap } from 'rxjs';
import { LoginRequest } from '../models/login-request.model';
import { AuthUser } from '../models/auth/auth-user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private currentUserSubject = new BehaviorSubject<AuthUser | null>(null);
  currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) { }

  registracija(data: RegisterRequest): Observable<any> {
    return this.http.post('/api/auth/registracija', data);
  }

  prijava(data: LoginRequest): Observable<AuthUser | null> {
  return this.http.post('/api/auth/prijava', data).pipe(
    switchMap(() => this.ucitajTrenutnogKorisnika())
  );
}

  trenutniKorisnik(): Observable<AuthUser> {
    return this.http.get<AuthUser>('/api/auth/trenutni-korisnik');
  }

  ucitajTrenutnogKorisnika(): Observable<AuthUser | null> {
    return this.trenutniKorisnik().pipe(
      tap((user) => {
        console.log('Trenutni korisnik:', user);
        this.currentUserSubject.next(user);
      }),
      catchError(() => {
        console.log('Nema trenutno prijavljenog korisnika.');
        this.currentUserSubject.next(null);
        return of(null);
      })
    );
  }

  getCurrentUser(): AuthUser | null {
    return this.currentUserSubject.value;
  }

  isLoggedIn(): boolean {
    return this.currentUserSubject.value !== null;
  }

  odjava(): Observable<any> {
  return this.http.post('/api/auth/odjava', {}).pipe(
    tap(() => {
      this.currentUserSubject.next(null);
    })
  );
}
}
