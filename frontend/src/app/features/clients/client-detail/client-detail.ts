import { DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Client } from '../../../core/models/client.models';
import { ClientService } from '../../../core/services/client.service';
import { NotificationService } from '../../../core/services/notification.service';
import { getErrorMessage } from '../../../shared/utils/http-error';

@Component({
  selector: 'app-client-detail',
  imports: [RouterLink, DatePipe],
  templateUrl: './client-detail.html',
  styleUrl: './client-detail.scss',
})
export class ClientDetail implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly clientService = inject(ClientService);
  private readonly notificationService = inject(NotificationService);

  readonly client = signal<Client | null>(null);

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.clientService.getById(id).subscribe({
      next: client => this.client.set(client),
      error: error => {
        // Ex. 404 « Le client 99 est introuvable. »
        this.notificationService.error(getErrorMessage(error));
        this.router.navigate(['/clients']);
      },
    });
  }
}