import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'sales' },
  {
    path: 'sales',
    loadChildren: () => import('./features/sales/sales.routes').then((m) => m.SALES_ROUTES),
  },
  { path: '**', redirectTo: 'sales' },
];
