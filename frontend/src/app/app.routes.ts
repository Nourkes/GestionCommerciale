import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { Login } from './features/auth/login/login';
import { ClientDetail } from './features/clients/client-detail/client-detail';
import { ClientForm } from './features/clients/client-form/client-form';
import { ClientList } from './features/clients/client-list/client-list';
import { ProductDetail } from './features/products/product-detail/product-detail';
import { ProductForm } from './features/products/product-form/product-form';
import { ProductList } from './features/products/product-list/product-list';
import { OrderDetail } from './features/orders/order-detail/order-detail';
import { OrderForm } from './features/orders/order-form/order-form';
import { OrderList } from './features/orders/order-list/order-list';
export const routes: Routes = [
  { path: 'login', component: Login },

  {
    path: '',
    canActivate: [authGuard],
    children: [
      // Clients ('new' AVANT ':id', sinon « new » serait pris pour un identifiant)
      { path: 'clients', component: ClientList },
      { path: 'clients/new', component: ClientForm },
      { path: 'clients/:id', component: ClientDetail },
      { path: 'clients/:id/edit', component: ClientForm },
      // Produits
      { path: 'products', component: ProductList },
      { path: 'products/new', component: ProductForm },
      { path: 'products/:id', component: ProductDetail },
      { path: 'products/:id/edit', component: ProductForm },
      { path: '', redirectTo: 'clients', pathMatch: 'full' },
            // Commandes
      { path: 'orders', component: OrderList },
      { path: 'orders/new', component: OrderForm },
      { path: 'orders/:id', component: OrderDetail },
      { path: 'orders/:id/edit', component: OrderForm },
    ],
  },

  { path: '**', redirectTo: '' },
];