import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './core/services/auth.service';
import { Navbar } from './layout/navbar/navbar';
import { ToastContainer } from './shared/components/toast-container/toast-container';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Navbar, ToastContainer],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected readonly authService = inject(AuthService);
}