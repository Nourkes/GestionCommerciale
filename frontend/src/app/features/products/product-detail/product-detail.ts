import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CURRENCY_CODE } from '../../../core/constants/app.constants';
import { Product } from '../../../core/models/product.models';
import { NotificationService } from '../../../core/services/notification.service';
import { ProductService } from '../../../core/services/product.service';
import { StockBadge } from '../../../shared/components/stock-badge/stock-badge';
import { getErrorMessage } from '../../../shared/utils/http-error';

@Component({
  selector: 'app-product-detail',
  imports: [RouterLink, CurrencyPipe, DatePipe, StockBadge],
  templateUrl: './product-detail.html',
  styleUrl: './product-detail.scss',
})
export class ProductDetail implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly productService = inject(ProductService);
  private readonly notificationService = inject(NotificationService);

  protected readonly currencyCode = CURRENCY_CODE;
  readonly product = signal<Product | null>(null);

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.productService.getById(id).subscribe({
      next: product => this.product.set(product),
      error: error => {
        this.notificationService.error(getErrorMessage(error));
        this.router.navigate(['/products']);
      },
    });
  }
}