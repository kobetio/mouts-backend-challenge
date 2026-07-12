import { inject } from '@angular/core';
import { CanDeactivateFn } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { map } from 'rxjs';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { SaleFormPageComponent } from '../pages/sale-form/sale-form-page.component';

export const unsavedChangesGuard: CanDeactivateFn<SaleFormPageComponent> = (component) => {
  if (!component.hasUnsavedChanges()) {
    return true;
  }

  const dialog = inject(MatDialog);

  return dialog
    .open(ConfirmDialogComponent, {
      width: '28rem',
      data: {
        title: 'Discard changes?',
        message: 'You have unsaved changes. Leave this page without saving?',
        confirmLabel: 'Leave',
        confirmColor: 'warn',
      },
    })
    .afterClosed()
    .pipe(map((confirmed) => !!confirmed));
};
