import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { CURRENCY_CODE, VAT_RATE } from '../../../core/constants/app.constants';
import { OrderDetail as OrderDetailModel } from '../../../core/models/order.models';
import { NotificationService } from '../../../core/services/notification.service';
import { OrderService } from '../../../core/services/order.service';
import { OrderStatusBadge } from '../../../shared/components/order-status-badge/order-status-badge';
import { getErrorMessage } from '../../../shared/utils/http-error';

@Component({
  selector: 'app-order-detail',
  imports: [RouterLink, CurrencyPipe, DatePipe, OrderStatusBadge],
  templateUrl: './order-detail.html',
  styleUrl: './order-detail.scss',
})
export class OrderDetail implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly orderService = inject(OrderService);
  private readonly notificationService = inject(NotificationService);

  protected readonly currencyCode = CURRENCY_CODE;
  protected readonly vatRatePercent = VAT_RATE * 100;

  readonly order = signal<OrderDetailModel | null>(null);
  readonly isProcessing = signal(false);

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.orderService.getById(id).subscribe({
      next: order => this.order.set(order),
      error: error => {
        this.notificationService.error(getErrorMessage(error));
        this.router.navigate(['/orders']);
      },
    });
  }

  validateOrder(order: OrderDetailModel): void {
    if (confirm(`Valider la commande ${order.orderNumber} ? Le stock des produits sera mis à jour.`)) {
      this.runAction(this.orderService.validate(order.id), 'Commande validée : le stock a été mis à jour.');
    }
  }

  cancelOrder(order: OrderDetailModel): void {
    const message = order.status === 'Validated'
      ? `Annuler la commande ${order.orderNumber} ? Les produits seront remis en stock.`
      : `Annuler la commande ${order.orderNumber} ?`;

    if (confirm(message)) {
      this.runAction(this.orderService.cancel(order.id), 'Commande annulée.');
    }
  }

  private runAction(action: Observable<OrderDetailModel>, successMessage: string): void {
    this.isProcessing.set(true);
    action.subscribe({
      next: updatedOrder => {
        this.order.set(updatedOrder); // le statut et les boutons se mettent à jour
        this.notificationService.success(successMessage);
        this.isProcessing.set(false);
      },
      error: error => {
        this.notificationService.error(getErrorMessage(error));
        this.isProcessing.set(false);
      },
    });
  }
}