// core/auth/auth.initializer.ts
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export function authInitializer() {
  return () => {
    const authService = inject(AuthService);
    authService.initializeAuth();
  };
}