import { Component, computed, input } from '@angular/core';
import { LOW_STOCK_THRESHOLD } from '../../../core/constants/app.constants';

@Component({
  selector: 'app-stock-badge',
  template: `<span class="badge rounded-pill {{ badge().cssClass }}">{{ badge().label }}</span>`,
})
export class StockBadge {
  // Valeur passée par le parent : <app-stock-badge [quantity]="product.stockQuantity" />
  readonly quantity = input.required<number>();

  protected readonly badge = computed(() => {
    const quantity = this.quantity();
    if (quantity === 0) {
      return { label: 'Rupture', cssClass: 'text-bg-danger' };
    }
    if (quantity <= LOW_STOCK_THRESHOLD) {
      return { label: `Faible (${quantity})`, cssClass: 'text-bg-warning' };
    }
    return { label: `${quantity} en stock`, cssClass: 'text-bg-success' };
  });
}