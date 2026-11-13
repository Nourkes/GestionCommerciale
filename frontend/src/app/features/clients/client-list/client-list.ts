import { DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Client } from '../../../core/models/client.models';
import { ClientService } from '../../../core/services/client.service';
import { NotificationService } from '../../../core/services/notification.service';
import { getErrorMessage } from '../../../shared/utils/http-error';

@Component({
  selector: 'app-client-list',
  imports: [RouterLink, DatePipe],
  templateUrl: './client-list.html',
  styleUrl: './client-list.scss',
})
export class ClientList implements OnInit {
  private readonly clientService = inject(ClientService);
  private readonly notificationService = inject(NotificationService);

  readonly clients = signal<Client[]>([]);
  readonly isLoading = signal(true);
  readonly searchTerm = signal('');

  // Liste filtrée par la recherche : recalculée automatiquement quand clients ou searchTerm changent
  readonly filteredClients = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    if (!term) {
      return this.clients();
    }
    return this.clients().filter(client =>
      [client.lastName, client.firstNameOrCompany, client.email, client.phone]
        .some(value => value.toLowerCase().includes(term))
    );
  });

  // Appelée automatiquement par Angular à l'ouverture de la page
  ngOnInit(): void {
    this.loadClients();
  }

  onSearch(event: Event): void {
    this.searchTerm.set((event.target as HTMLInputElement).value);
  }

  getFullName(client: Client): string {
    return `${client.lastName} ${client.firstNameOrCompany}`.trim();
  }

  deleteClient(client: Client): void {
    if (!confirm(`Supprimer le client « ${this.getFullName(client)} » ?`)) {
      return;
    }

    this.clientService.delete(client.id).subscribe({
      next: () => {
        // On retire le client de la liste affichée, sans recharger toute la page
        this.clients.update(list => list.filter(c => c.id !== client.id));
        this.notificationService.success('Client supprimé avec succès.');
      },
      // Ex. 409 « Impossible de supprimer ce client : il possède des commandes. »
      error: error => this.notificationService.error(getErrorMessage(error)),
    });
  }

  private loadClients(): void {
    this.isLoading.set(true);
    this.clientService.getAll().subscribe({
      next: clients => {
        this.clients.set(clients);
        this.isLoading.set(false);
      },
      error: error => {
        this.notificationService.error(getErrorMessage(error));
        this.isLoading.set(false);
      },
    });
  }
}