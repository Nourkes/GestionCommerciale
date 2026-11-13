import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CURRENCY_CODE } from '../../../core/constants/app.constants';
import { SaveProduct } from '../../../core/models/product.models';
import { NotificationService } from '../../../core/services/notification.service';
import { ProductService } from '../../../core/services/product.service';
import { getFieldErrorMessage } from '../../../shared/utils/form-errors';
import { getErrorMessage } from '../../../shared/utils/http-error';

@Component({
  selector: 'app-product-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './product-form.html',
  styleUrl: './product-form.scss',
})
export class ProductForm implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly productService = inject(ProductService);
  private readonly notificationService = inject(NotificationService);

  protected readonly currencyCode = CURRENCY_CODE;

  private readonly productId = signal<number | null>(null);
  readonly isEditMode = computed(() => this.productId() !== null);
  readonly isLoading = signal(false);
  readonly isSaving = signal(false);

  readonly productForm = this.formBuilder.nonNullable.group({
    reference: ['', [Validators.required, Validators.maxLength(50)]],
    name: ['', [Validators.required, Validators.maxLength(150)]],
    description: ['', [Validators.maxLength(500)]],
    unitPriceHT: [0, [Validators.required, Validators.min(0)]],
    stockQuantity: [0, [Validators.required, Validators.min(0), Validators.pattern(/^\d+$/)]], // entier
  });

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.productId.set(Number(idParam));
      this.loadProduct(Number(idParam));
    }
  }

  fieldError(fieldName: keyof SaveProduct): string | null {
    return getFieldErrorMessage(this.productForm.controls[fieldName]);
  }

  onSubmit(): void {
    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return;
    }

    this.isSaving.set(true);
    const product: SaveProduct = this.productForm.getRawValue();
    const id = this.productId();

    const request = id === null
      ? this.productService.create(product)
      : this.productService.update(id, product);

    request.subscribe({
      next: () => {
        this.notificationService.success(id === null ? 'Produit créé avec succès.' : 'Produit modifié avec succès.');
        this.router.navigate(['/products']);
      },
      error: error => {
        this.notificationService.error(getErrorMessage(error));
        this.isSaving.set(false);
      },
    });
  }

  private loadProduct(id: number): void {
    this.isLoading.set(true);
    this.productService.getById(id).subscribe({
      next: product => {
        this.productForm.patchValue({ ...product, description: product.description ?? '' });
        this.isLoading.set(false);
      },
      error: error => {
        this.notificationService.error(getErrorMessage(error));
        this.router.navigate(['/products']);
      },
    });
  }
}