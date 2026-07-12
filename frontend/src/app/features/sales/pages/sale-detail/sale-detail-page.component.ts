import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiClientError } from '../../../../core/models/api-response.model';
import { ConfirmDialogComponent } from '../../../../shared/components/confirm-dialog/confirm-dialog.component';
import { SaleItemsTableComponent } from '../../components/sale-items-table/sale-items-table.component';
import {
  Sale,
  SaleItem,
  isSaleCancelled,
  saleStatusLabel,
} from '../../models/sale.model';
import { SalesApiService } from '../../services/sales-api.service';

@Component({
  selector: 'app-sale-detail-page',
  imports: [
    RouterLink,
    MatButtonModule,
    MatCardModule,
    MatChipsModule,
    MatIconModule,
    SaleItemsTableComponent,
  ],
  templateUrl: './sale-detail-page.component.html',
  styleUrl: './sale-detail-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SaleDetailPageComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly salesApi = inject(SalesApiService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  readonly sale = signal<Sale | null>(null);
  readonly loadError = signal<string | null>(null);

  constructor() {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      this.loadError.set('Sale Id is required.');
      return;
    }

    this.loadSale(id);
  }

  get isCancelled(): boolean {
    const current = this.sale();
    return current ? isSaleCancelled(current.status) : false;
  }

  statusLabel(): string {
    const current = this.sale();
    return current ? saleStatusLabel(current.status) : '';
  }

  formatDate(value: string): string {
    return new Date(value).toLocaleString();
  }

  formatCurrency(value: number): string {
    return value.toLocaleString(undefined, { style: 'currency', currency: 'USD' });
  }

  onCancelSale(): void {
    const current = this.sale();
    if (!current) {
      return;
    }

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '28rem',
      data: {
        title: 'Cancel sale',
        message: `Cancel sale #${current.saleNumber}?`,
        confirmLabel: 'Cancel sale',
        confirmColor: 'warn',
      },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (!confirmed) {
        return;
      }

      this.salesApi.cancelSale(current.id).subscribe({
        next: (updated) => {
          this.sale.set(updated);
          this.showSuccess('Sale cancelled successfully');
        },
        error: (error) => this.showError(error),
      });
    });
  }

  onCancelItem(item: SaleItem): void {
    const current = this.sale();
    if (!current) {
      return;
    }

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '28rem',
      data: {
        title: 'Cancel item',
        message: `Cancel "${item.product.name}" from sale #${current.saleNumber}?`,
        confirmLabel: 'Cancel item',
        confirmColor: 'warn',
      },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (!confirmed) {
        return;
      }

      this.salesApi.cancelItem(current.id, item.id).subscribe({
        next: (updated) => {
          this.sale.set(updated);
          this.showSuccess('Sale item cancelled successfully');
        },
        error: (error) => this.showError(error),
      });
    });
  }

  onDeleteSale(): void {
    const current = this.sale();
    if (!current) {
      return;
    }

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '28rem',
      data: {
        title: 'Delete sale',
        message: `Permanently delete sale #${current.saleNumber}?`,
        confirmLabel: 'Delete',
        confirmColor: 'warn',
      },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (!confirmed) {
        return;
      }

      this.salesApi.delete(current.id).subscribe({
        next: () => {
          this.showSuccess('Sale deleted successfully');
          void this.router.navigate(['/sales']);
        },
        error: (error) => this.showError(error),
      });
    });
  }

  private loadSale(id: string): void {
    this.salesApi.getById(id).subscribe({
      next: (sale) => this.sale.set(sale),
      error: (error) => this.loadError.set(this.resolveErrorMessage(error)),
    });
  }

  private showSuccess(message: string): void {
    this.snackBar.open(message, 'Close', { duration: 4000 });
  }

  private showError(error: unknown): void {
    this.snackBar.open(this.resolveErrorMessage(error), 'Close', { duration: 6000 });
  }

  private resolveErrorMessage(error: unknown): string {
    if (error instanceof ApiClientError) {
      return error.message;
    }

    return 'An unexpected error occurred.';
  }
}
