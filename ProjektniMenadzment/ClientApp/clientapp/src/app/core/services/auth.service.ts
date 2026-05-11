import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap, catchError, of } from 'rxjs';
import { RegisterRequest } from '../models/register-request.model';
import { LoginRequest } from '../models/login-request.model';
import { AuthUser } from '../models/auth/auth-user.model';

interface LoginResponse {
  token: string;
  korisnik: AuthUser;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private readonly TOKEN_KEY = 'auth_token';

  private currentUserSubject = new BehaviorSubject<AuthUser | null>(
    this.getUserFromToken()   // restore state on page refresh
  );
  currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {}

  registracija(data: RegisterRequest): Observable<any> {
    return this.http.post('/api/auth/registracija', data);
  }

  prijava(data: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>('/api/auth/prijava', data).pipe(
      tap(response => {
        localStorage.setItem(this.TOKEN_KEY, response.token);
        this.currentUserSubject.next(response.korisnik);
      })
    );
  }

  odjava(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    this.currentUserSubject.next(null);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    if (!token) return false;
    return !this.isTokenExpired(token);
  }

  getCurrentUser(): AuthUser | null {
    return this.currentUserSubject.value;
  }

  isAdmin(): boolean {
    return this.currentUserSubject.value?.isAdmin ?? false;
  }

  // Called on app startup to rehydrate user state from stored token
  initializeAuth(): void {
    const token = this.getToken();
    if (!token || this.isTokenExpired(token)) {
      this.odjava();
      return;
    }
    const user = this.getUserFromToken();
    this.currentUserSubject.next(user);
  }

  // -------------------------------------------------------
  // Private helpers
  // -------------------------------------------------------

  private getUserFromToken(): AuthUser | null {
    const token = localStorage.getItem(this.TOKEN_KEY);
    if (!token) return null;
    try {
      const payload = this.decodeToken(token);
      if (!payload || this.isTokenExpired(token)) return null;

      return {
        korisnickoIme: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
        email: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
        isAdmin: this.hasRole(payload, 'Administrator'),
        isPM: this.hasRole(payload, 'ProjektniMenadzer')
      };
    } catch {
      return null;
    }
  }

  private decodeToken(token: string): any {
    try {
      const payload = token.split('.')[1];
      return JSON.parse(atob(payload));
    } catch {
      return null;
    }
  }

  private isTokenExpired(token: string): boolean {
    const payload = this.decodeToken(token);
    if (!payload?.exp) return true;
    return Date.now() >= payload.exp * 1000;
  }

  private hasRole(payload: any, role: string): boolean {
    const roleClaim = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    if (!roleClaim) return false;
    // Role claim can be a string (one role) or array (multiple roles)
    return Array.isArray(roleClaim)
      ? roleClaim.includes(role)
      : roleClaim === role;
  }
}