import { CurrencyPipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { CURRENCY_CODE, VAT_RATE } from '../../../core/constants/app.constants';
import { Client } from '../../../core/models/client.models';
import { OrderDetail, SaveOrder } from '../../../core/models/order.models';
import { Product } from '../../../core/models/product.models';
import { ClientService } from '../../../core/services/client.service';
import { NotificationService } from '../../../core/services/notification.service';
import { OrderService } from '../../../core/services/order.service';
import { ProductService } from '../../../core/services/product.service';
import { getFieldErrorMessage } from '../../../shared/utils/form-errors';
import { getErrorMessage } from '../../../shared/utils/http-error';

@Component({
  selector: 'app-order-form',
  imports: [ReactiveFormsModule, RouterLink, CurrencyPipe],
  templateUrl: './order-form.html',
  styleUrl: './order-form.scss',
})
export class OrderForm implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly orderService = inject(OrderService);
  private readonly clientService = inject(ClientService);
  private readonly productService = inject(ProductService);
  private readonly notificationService = inject(NotificationService);

  protected readonly currencyCode = CURRENCY_CODE;
  protected readonly vatRatePercent = VAT_RATE * 100;

  private readonly orderId = signal<number | null>(null);
  readonly isEditMode = computed(() => this.orderId() !== null);
  readonly isLoading = signal(true);
  readonly isSaving = signal(false);

  // Données nécessaires aux listes déroulantes
  readonly clients = signal<Client[]>([]);
  readonly products = signal<Product[]>([]);

  // Le formulaire : client + date + une liste de lignes 
  readonly orderForm = this.formBuilder.group({
    clientId: this.formBuilder.control<number | null>(null, Validators.required),
    orderDate: this.formBuilder.nonNullable.control(this.today(), Validators.required),
    lines: this.formBuilder.array([this.createLine()], Validators.minLength(1)),
  });

  get lines() {
    return this.orderForm.controls.lines;
  }

  // Calculs en direct 
  private readonly formValue = toSignal(this.orderForm.valueChanges, { initialValue: this.orderForm.value });

  private readonly productsById = computed(() => new Map(this.products().map(p => [p.id, p])));

  // Pour chaque ligne : le produit choisi, son prix, le total de la ligne, et un éventuel dépassement de stock
  readonly lineSummaries = computed(() =>
    (this.formValue().lines ?? []).map(line => {
      const product = line.productId != null ? this.productsById().get(line.productId) : undefined;
      const quantity = Number(line.quantity) || 0;
      const unitPrice = product?.unitPriceHT ?? 0;
      return {
        product,
        unitPrice,
        lineTotal: quantity * unitPrice,                                  // Quantité × Prix unitaire
        exceedsStock: !!product && quantity > product.stockQuantity,
      };
    })
  );

  readonly totalHT = computed(() => this.lineSummaries().reduce((sum, line) => sum + line.lineTotal, 0));
  readonly totalVAT = computed(() => this.totalHT() * VAT_RATE);
  readonly totalTTC = computed(() => this.totalHT() + this.totalVAT());
  readonly hasStockIssue = computed(() => this.lineSummaries().some(line => line.exceedsStock));

  // ───── Chargement ─────
  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.orderId.set(Number(idParam));
    }

    // Clients et produits chargés en parallèle 
    forkJoin({
      clients: this.clientService.getAll(),
      products: this.productService.getAll(),
    }).subscribe({
      next: ({ clients, products }) => {
        this.clients.set(clients);
        this.products.set(products);

        const id = this.orderId();
        if (id === null) {
          this.isLoading.set(false);
        } else {
          this.loadOrder(id);
        }
      },
      error: error => {
        this.notificationService.error(getErrorMessage(error));
        this.router.navigate(['/orders']);
      },
    });
  }

  //Gestion des lignes
  addLine(): void {
    this.lines.push(this.createLine());
  }

  removeLine(index: number): void {
    if (this.lines.length > 1) {
      this.lines.removeAt(index);
    }
  }

  /** Empêche de choisir deux fois le même produit dans deux lignes différentes. */
  isProductSelectedInOtherLine(productId: number, lineIndex: number): boolean {
    return this.lines.controls.some((line, index) => index !== lineIndex && line.controls.productId.value === productId);
  }

  clientError(): string | null {
    return getFieldErrorMessage(this.orderForm.controls.clientId);
  }

  dateError(): string | null {
    return getFieldErrorMessage(this.orderForm.controls.orderDate);
  }

  lineError(index: number, field: 'productId' | 'quantity'): string | null {
    return getFieldErrorMessage(this.lines.at(index).controls[field]);
  }

  //Enregistrement 
  onSubmit(): void {
    if (this.orderForm.invalid) {
      this.orderForm.markAllAsTouched();
      return;
    }
    if (this.hasStockIssue()) {
      this.notificationService.error('Une ou plusieurs quantités dépassent le stock disponible.');
      return;
    }

    const formValue = this.orderForm.getRawValue();
    const order: SaveOrder = {
      clientId: formValue.clientId!,
      orderDate: formValue.orderDate,
      lines: formValue.lines.map(line => ({ productId: line.productId!, quantity: Number(line.quantity) })),
    };

    this.isSaving.set(true);
    const id = this.orderId();
    const request = id === null
      ? this.orderService.create(order)
      : this.orderService.update(id, order);

    request.subscribe({
      next: savedOrder => {
        this.notificationService.success(
          id === null ? `Commande ${savedOrder.orderNumber} créée.` : `Commande ${savedOrder.orderNumber} modifiée.`
        );
        this.router.navigate(['/orders', savedOrder.id]); // on ouvre le détail pour pouvoir la valider
      },
      error: error => {
        this.notificationService.error(getErrorMessage(error));
        this.isSaving.set(false);
      },
    });
  }

  private createLine(productId: number | null = null, quantity = 1) {
    return this.formBuilder.group({
      productId: this.formBuilder.control<number | null>(productId, Validators.required),
      quantity: this.formBuilder.nonNullable.control(quantity, [
        Validators.required,
        Validators.min(1),
        Validators.pattern(/^\d+$/), // nombre entier
      ]),
    });
  }

  private loadOrder(id: number): void {
    this.orderService.getById(id).subscribe({
      next: order => this.fillForm(order),
      error: error => {
        this.notificationService.error(getErrorMessage(error));
        this.router.navigate(['/orders']);
      },
    });
  }

  private fillForm(order: OrderDetail): void {
    if (order.status !== 'Draft') {
      this.notificationService.error('Seule une commande au statut Brouillon peut être modifiée.');
      this.router.navigate(['/orders', order.id]);
      return;
    }

    this.lines.clear();
    order.lines.forEach(line => this.lines.push(this.createLine(line.productId, line.quantity)));
    this.orderForm.patchValue({
      clientId: order.clientId,
      orderDate: order.orderDate.substring(0, 10), // "2026-11-12T00:00:00" → "2026-11-12"
    });
    this.isLoading.set(false);
  }

  private today(): string {
    return new Date().toISOString().substring(0, 10); 
  }
}