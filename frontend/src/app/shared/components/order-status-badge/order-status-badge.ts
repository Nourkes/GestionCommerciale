import { Component, computed, input } from '@angular/core';
import { ORDER_STATUS_LABELS } from '../../../core/constants/app.constants';
import { OrderStatus } from '../../../core/models/order.models';

@Component({
  selector: 'app-order-status-badge',
  template: `<span class="badge rounded-pill {{ cssClass() }}">{{ label() }}</span>`,
})
export class OrderStatusBadge {
  readonly status = input.required<OrderStatus>();

  private readonly cssClasses: Record<OrderStatus, string> = {
    Draft: 'text-bg-secondary',
    Validated: 'text-bg-success',
    Cancelled: 'text-bg-danger',
  };

  protected readonly label = computed(() => ORDER_STATUS_LABELS[this.status()]);
  protected readonly cssClass = computed(() => this.cssClasses[this.status()]);
}