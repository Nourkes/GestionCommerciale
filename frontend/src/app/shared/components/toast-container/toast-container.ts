import { Component, inject } from '@angular/core';
import { NotificationType } from '../../../core/models/notification.models';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-toast-container',
  templateUrl: './toast-container.html',
  styleUrl: './toast-container.scss',
})
export class ToastContainer {
  protected readonly notificationService = inject(NotificationService);

  // Icône associée à chaque type de message
  protected readonly icons: Record<NotificationType, string> = {
    success: 'bi-check-circle-fill',
    error: 'bi-x-circle-fill',
    info: 'bi-info-circle-fill',
  };
}