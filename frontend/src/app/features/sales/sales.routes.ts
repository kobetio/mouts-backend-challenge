import { Routes } from '@angular/router';
import { unsavedChangesGuard } from './guards/unsaved-changes.guard';

export const SALES_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/sales-list/sales-list-page.component').then(
        (m) => m.SalesListPageComponent,
      ),
  },
  {
    path: 'new',
    loadComponent: () =>
      import('./pages/sale-form/sale-form-page.component').then((m) => m.SaleFormPageComponent),
  },
  {
    path: ':id/edit',
    loadComponent: () =>
      import('./pages/sale-form/sale-form-page.component').then((m) => m.SaleFormPageComponent),
    canDeactivate: [unsavedChangesGuard],
  },
  {
    path: ':id',
    loadComponent: () =>
      import('./pages/sale-detail/sale-detail-page.component').then(
        (m) => m.SaleDetailPageComponent,
      ),
  },
];
