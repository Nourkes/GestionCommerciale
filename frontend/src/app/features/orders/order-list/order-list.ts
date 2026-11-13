import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CURRENCY_CODE, ORDER_STATUS_LABELS } from '../../../core/constants/app.constants';
import { OrderStatus, OrderSummary } from '../../../core/models/order.models';
import { NotificationService } from '../../../core/services/notification.service';
import { OrderService } from '../../../core/services/order.service';
import { OrderStatusBadge } from '../../../shared/components/order-status-badge/order-status-badge';
import { getErrorMessage } from '../../../shared/utils/http-error';

@Component({
  selector: 'app-order-list',
  imports: [RouterLink, CurrencyPipe, DatePipe, OrderStatusBadge],
  templateUrl: './order-list.html',
  styleUrl: './order-list.scss',
})
export class OrderList implements OnInit {
  private readonly orderService = inject(OrderService);
  private readonly notificationService = inject(NotificationService);

  protected readonly currencyCode = CURRENCY_CODE;
  protected readonly statusOptions = Object.entries(ORDER_STATUS_LABELS) as [OrderStatus, string][];

  readonly orders = signal<OrderSummary[]>([]);
  readonly isLoading = signal(true);
  readonly searchTerm = signal('');
  readonly statusFilter = signal<OrderStatus | ''>('');

  readonly filteredOrders = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    const status = this.statusFilter();

    return this.orders().filter(order =>
      (!status || order.status === status) &&
      (!term || order.orderNumber.toLowerCase().includes(term) || order.clientName.toLowerCase().includes(term))
    );
  });

  ngOnInit(): void {
    this.loadOrders();
  }

  onSearch(event: Event): void {
    this.searchTerm.set((event.target as HTMLInputElement).value);
  }

  onStatusChange(event: Event): void {
    this.statusFilter.set((event.target as HTMLSelectElement).value as OrderStatus | '');
  }

  deleteOrder(order: OrderSummary): void {
    if (!confirm(`Supprimer la commande ${order.orderNumber} ?`)) {
      return;
    }

    this.orderService.delete(order.id).subscribe({
      next: () => {
        this.orders.update(list => list.filter(o => o.id !== order.id));
        this.notificationService.success('Commande supprimée avec succès.');
      },
      error: error => this.notificationService.error(getErrorMessage(error)),
    });
  }

  private loadOrders(): void {
    this.isLoading.set(true);
    this.orderService.getAll().subscribe({
      next: orders => {
        this.orders.set(orders);
        this.isLoading.set(false);
      },
      error: error => {
        this.notificationService.error(getErrorMessage(error));
        this.isLoading.set(false);
      },
    });
  }
}