import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const managerGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const korisnik = authService.getCurrentUser();

  if (korisnik?.isAdmin || korisnik?.isPM) {
    return true;
  }

  return router.createUrlTree(['/dashboard']);
};
