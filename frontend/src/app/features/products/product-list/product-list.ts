import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CURRENCY_CODE } from '../../../core/constants/app.constants';
import { Product } from '../../../core/models/product.models';
import { NotificationService } from '../../../core/services/notification.service';
import { ProductService } from '../../../core/services/product.service';
import { StockBadge } from '../../../shared/components/stock-badge/stock-badge';
import { getErrorMessage } from '../../../shared/utils/http-error';

@Component({
  selector: 'app-product-list',
  imports: [RouterLink, CurrencyPipe, DatePipe, StockBadge],
  templateUrl: './product-list.html',
  styleUrl: './product-list.scss',
})
export class ProductList implements OnInit {
  private readonly productService = inject(ProductService);
  private readonly notificationService = inject(NotificationService);

  protected readonly currencyCode = CURRENCY_CODE;

  readonly products = signal<Product[]>([]);
  readonly isLoading = signal(true);
  readonly searchTerm = signal('');

  readonly filteredProducts = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    if (!term) {
      return this.products();
    }
    return this.products().filter(product =>
      [product.reference, product.name, product.description ?? '']
        .some(value => value.toLowerCase().includes(term))
    );
  });

  ngOnInit(): void {
    this.loadProducts();
  }

  onSearch(event: Event): void {
    this.searchTerm.set((event.target as HTMLInputElement).value);
  }

  deleteProduct(product: Product): void {
    if (!confirm(`Supprimer le produit « ${product.reference} - ${product.name} » ?`)) {
      return;
    }

    this.productService.delete(product.id).subscribe({
      next: () => {
        this.products.update(list => list.filter(p => p.id !== product.id));
        this.notificationService.success('Produit supprimé avec succès.');
      },
      error: error => this.notificationService.error(getErrorMessage(error)),
    });
  }

  private loadProducts(): void {
    this.isLoading.set(true);
    this.productService.getAll().subscribe({
      next: products => {
        this.products.set(products);
        this.isLoading.set(false);
      },
      error: error => {
        this.notificationService.error(getErrorMessage(error));
        this.isLoading.set(false);
      },
    });
  }
}