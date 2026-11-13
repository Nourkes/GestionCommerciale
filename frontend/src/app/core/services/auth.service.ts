import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginRequest, LoginResponse } from '../models/auth.models';

// Clés utilisées dans le localStorage du navigateur
const TOKEN_KEY = 'gc_token';
const EXPIRES_AT_KEY = 'gc_expires_at';
const USERNAME_KEY = 'gc_username';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly apiUrl = `${environment.apiUrl}/auth`;

  private readonly currentUsername = signal<string | null>(localStorage.getItem(USERNAME_KEY));
  readonly username = this.currentUsername.asReadonly();

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${this.apiUrl}/login`, request)
      .pipe(tap(response => this.saveSession(response)));
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(EXPIRES_AT_KEY);
    localStorage.removeItem(USERNAME_KEY);
    this.currentUsername.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    const expiresAt = localStorage.getItem(EXPIRES_AT_KEY);
    return !!token && !!expiresAt && new Date(expiresAt) > new Date();
  }

  private saveSession(response: LoginResponse): void {
    localStorage.setItem(TOKEN_KEY, response.token);
    localStorage.setItem(EXPIRES_AT_KEY, response.expiresAt);
    localStorage.setItem(USERNAME_KEY, response.username);
    this.currentUsername.set(response.username);
  }
}