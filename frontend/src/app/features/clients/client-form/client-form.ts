import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { SaveClient } from '../../../core/models/client.models';
import { ClientService } from '../../../core/services/client.service';
import { NotificationService } from '../../../core/services/notification.service';
import { getFieldErrorMessage } from '../../../shared/utils/form-errors';
import { getErrorMessage } from '../../../shared/utils/http-error';

@Component({
  selector: 'app-client-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './client-form.html',
  styleUrl: './client-form.scss',
})
export class ClientForm implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly clientService = inject(ClientService);
  private readonly notificationService = inject(NotificationService);

  private readonly clientId = signal<number | null>(null);
  readonly isEditMode = computed(() => this.clientId() !== null);

  readonly isLoading = signal(false);
  readonly isSaving = signal(false);

  readonly clientForm = this.formBuilder.nonNullable.group({
    lastName: ['', [Validators.required, Validators.maxLength(100)]],
    firstNameOrCompany: ['', [Validators.required, Validators.maxLength(150)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(150)]],
    phone: ['', [Validators.maxLength(20), Validators.pattern(/^[+\d\s-]*$/)]],
    address: ['', [Validators.maxLength(250)]],
  });

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.clientId.set(Number(idParam));
      this.loadClient(Number(idParam));
    }
  }

  fieldError(fieldName: keyof SaveClient): string | null {
    return getFieldErrorMessage(this.clientForm.controls[fieldName]);
  }

  onSubmit(): void {
    if (this.clientForm.invalid) {
      this.clientForm.markAllAsTouched(); 
      return;
    }

    this.isSaving.set(true);
    const client: SaveClient = this.clientForm.getRawValue();
    const id = this.clientId();

    const request = id === null
      ? this.clientService.create(client)
      : this.clientService.update(id, client);

    request.subscribe({
      next: () => {
        this.notificationService.success(id === null ? 'Client créé avec succès.' : 'Client modifié avec succès.');
        this.router.navigate(['/clients']);
      },
      error: error => {
        this.notificationService.error(getErrorMessage(error));
        this.isSaving.set(false);
      },
    });
  }

  private loadClient(id: number): void {
    this.isLoading.set(true);
    this.clientService.getById(id).subscribe({
      next: client => {
        this.clientForm.patchValue(client); // remplit les champs avec les valeurs existantes
        this.isLoading.set(false);
      },
      error: error => {
        this.notificationService.error(getErrorMessage(error));
        this.router.navigate(['/clients']); // client introuvable → retour à la liste
      },
    });
  }
}