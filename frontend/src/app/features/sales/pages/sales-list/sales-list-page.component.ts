import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PageEvent } from '@angular/material/paginator';
import { Sort } from '@angular/material/sort';
import { RouterLink } from '@angular/router';
import { ApiClientError } from '../../../../core/models/api-response.model';
import { ConfirmDialogComponent } from '../../../../shared/components/confirm-dialog/confirm-dialog.component';
import { SaleFiltersComponent } from '../../components/sale-filters/sale-filters.component';
import { SalesTableComponent } from '../../components/sales-table/sales-table.component';
import { Sale, SaleListQuery } from '../../models/sale.model';
import { SalesApiService } from '../../services/sales-api.service';

@Component({
  selector: 'app-sales-list-page',
  imports: [
    RouterLink,
    MatButtonModule,
    MatIconModule,
    SaleFiltersComponent,
    SalesTableComponent,
  ],
  templateUrl: './sales-list-page.component.html',
  styleUrl: './sales-list-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SalesListPageComponent {
  private readonly salesApi = inject(SalesApiService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  readonly sales = signal<Sale[]>([]);
  readonly totalItems = signal(0);
  readonly pageIndex = signal(0);
  readonly pageSize = signal(10);
  readonly sortActive = signal('saleDate');
  readonly sortDirection = signal<'asc' | 'desc'>('desc');
  readonly filters = signal<SaleListQuery>({});
  readonly loadError = signal<string | null>(null);
  readonly initialLoadDone = signal(false);

  constructor() {
    this.loadSales();
  }

  get isEmpty(): boolean {
    return this.initialLoadDone() && this.sales().length === 0 && !this.loadError();
  }

  onFiltersApply(query: SaleListQuery): void {
    this.filters.set(query);
    this.pageIndex.set(0);
    this.loadSales();
  }

  onFiltersReset(): void {
    this.filters.set({});
    this.pageIndex.set(0);
    this.loadSales();
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
    this.loadSales();
  }

  onSortChange(sort: Sort): void {
    if (!sort.active || !sort.direction) {
      this.sortActive.set('saleDate');
      this.sortDirection.set('desc');
    } else {
      this.sortActive.set(sort.active);
      this.sortDirection.set(sort.direction);
    }

    this.loadSales();
  }

  onDeleteSale(sale: Sale): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '28rem',
      data: {
        title: 'Delete sale',
        message: `Permanently delete sale #${sale.saleNumber}? This cannot be undone.`,
        confirmLabel: 'Delete',
        confirmColor: 'warn',
      },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (!confirmed) {
        return;
      }

      this.salesApi.delete(sale.id).subscribe({
        next: () => {
          this.showSuccess('Sale deleted successfully');
          this.loadSales();
        },
        error: (error) => this.showError(error),
      });
    });
  }

  onCancelSale(sale: Sale): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '28rem',
      data: {
        title: 'Cancel sale',
        message: `Cancel sale #${sale.saleNumber}? The sale will remain visible for audit purposes.`,
        confirmLabel: 'Cancel sale',
        confirmColor: 'warn',
      },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (!confirmed) {
        return;
      }

      this.salesApi.cancelSale(sale.id).subscribe({
        next: () => {
          this.showSuccess('Sale cancelled successfully');
          this.loadSales();
        },
        error: (error) => this.showError(error),
      });
    });
  }

  private loadSales(): void {
    this.loadError.set(null);

    const query: SaleListQuery = {
      ...this.filters(),
      page: this.pageIndex() + 1,
      size: this.pageSize(),
      orderBy: `${this.sortActive()} ${this.sortDirection()}`,
    };

    this.salesApi.list(query).subscribe({
      next: (result) => {
        this.sales.set(result.items);
        this.totalItems.set(result.totalItems);
        this.initialLoadDone.set(true);
      },
      error: (error) => {
        this.loadError.set(this.resolveErrorMessage(error));
        this.initialLoadDone.set(true);
      },
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
