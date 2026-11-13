import { Component, computed, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss',
})
export class Navbar {
  protected readonly authService = inject(AuthService);

  // Première lettre du nom, affichée dans l'avatar (ex. « A » pour admin)
  protected readonly userInitial = computed(
    () => (this.authService.username() ?? '?').charAt(0).toUpperCase()
  );

  logout(): void {
    this.authService.logout();
  }
}