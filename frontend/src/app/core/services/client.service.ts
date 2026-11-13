import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Client, SaveClient } from '../models/client.models';

@Injectable({ providedIn: 'root' })
export class ClientService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/clients`;

  getAll(): Observable<Client[]> {
    return this.http.get<Client[]>(this.apiUrl);
  }

  getById(id: number): Observable<Client> {
    return this.http.get<Client>(`${this.apiUrl}/${id}`);
  }

  create(client: SaveClient): Observable<Client> {
    return this.http.post<Client>(this.apiUrl, client);
  }

  update(id: number, client: SaveClient): Observable<Client> {
    return this.http.put<Client>(`${this.apiUrl}/${id}`, client);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}