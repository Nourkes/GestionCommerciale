import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { NotificationService } from '../services/notification.service';

/**
 * Exécuté automatiquement pour CHAQUE requête HTTP envoyée par Angular.
 * 1. Ajoute le token JWT dans l'en-tête Authorization.
 * 2. Si l'API répond 401 (token absent, expiré ou invalide), déconnecte l'utilisateur.
 */
export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const authService = inject(AuthService);
  const notificationService = inject(NotificationService);
  const token = authService.getToken();

  const authorizedRequest = token
    ? request.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : request;

  return next(authorizedRequest).pipe(
    catchError((error: HttpErrorResponse) => {
      const isLoginRequest = request.url.endsWith('/auth/login');
      if (error.status === 401 && !isLoginRequest) {
        notificationService.info('Votre session a expiré. Veuillez vous reconnecter.');
        authService.logout();
      }
      return throwError(() => error);
    })
  );
};