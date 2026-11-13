import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/** Laisse entrer sur une page si l'utilisateur est connecté, sinon redirige vers /login. */
export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.isLoggedIn() ? true : router.createUrlTree(['/login']);
};