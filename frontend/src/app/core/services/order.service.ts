import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { OrderDetail, OrderSummary, SaveOrder } from '../models/order.models';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/orders`;

  getAll(): Observable<OrderSummary[]> {
    return this.http.get<OrderSummary[]>(this.apiUrl);
  }

  getById(id: number): Observable<OrderDetail> {
    return this.http.get<OrderDetail>(`${this.apiUrl}/${id}`);
  }

  create(order: SaveOrder): Observable<OrderDetail> {
    return this.http.post<OrderDetail>(this.apiUrl, order);
  }

  update(id: number, order: SaveOrder): Observable<OrderDetail> {
    return this.http.put<OrderDetail>(`${this.apiUrl}/${id}`, order);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  validate(id: number): Observable<OrderDetail> {
    return this.http.post<OrderDetail>(`${this.apiUrl}/${id}/validate`, {});
  }

  cancel(id: number): Observable<OrderDetail> {
    return this.http.post<OrderDetail>(`${this.apiUrl}/${id}/cancel`, {});
  }
}