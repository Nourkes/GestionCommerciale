import { Injectable, signal } from '@angular/core';
import { AppNotification, NotificationType } from '../models/notification.models';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private readonly items = signal<AppNotification[]>([]);
  readonly notifications = this.items.asReadonly();

  private nextId = 1;

  success(message: string): void {
    this.show('success', message, 4000);
  }

  error(message: string): void {
    this.show('error', message, 6000); 
  }

  info(message: string): void {
    this.show('info', message, 4000);
  }

  dismiss(id: number): void {
    this.items.update(list => list.filter(notification => notification.id !== id));
  }

  private show(type: NotificationType, message: string, durationMs: number): void {
    const id = this.nextId++;
    this.items.update(list => [...list, { id, type, message }]);
    setTimeout(() => this.dismiss(id), durationMs); 
  }
}
