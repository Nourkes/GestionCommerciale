import { registerLocaleData } from '@angular/common';
import localeFr from '@angular/common/locales/fr';
import { ApplicationConfig, LOCALE_ID, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';

// Charge les règles de formatage françaises (virgule décimale, espaces des milliers...)
registerLocaleData(localeFr);

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    { provide: LOCALE_ID, useValue: 'fr' },   // ← français par défaut pour tous les pipes
  ],
};