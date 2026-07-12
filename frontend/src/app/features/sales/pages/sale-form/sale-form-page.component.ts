import { ChangeDetectionStrategy, Component, ViewChild, inject, signal } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiClientError } from '../../../../core/models/api-response.model';
import {
  SaleFormComponent,
  SaleFormSubmitValue,
} from '../../components/sale-form/sale-form.component';
import { Sale } from '../../models/sale.model';
import { SalesApiService } from '../../services/sales-api.service';

@Component({
  selector: 'app-sale-form-page',
  imports: [SaleFormComponent],
  templateUrl: './sale-form-page.component.html',
  styleUrl: './sale-form-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SaleFormPageComponent {
  @ViewChild(SaleFormComponent) saleForm?: SaleFormComponent;

  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly salesApi = inject(SalesApiService);
  private readonly snackBar = inject(MatSnackBar);

  readonly sale = signal<Sale | null>(null);
  readonly loadError = signal<string | null>(null);
  readonly submitting = signal(false);

  readonly saleId = this.route.snapshot.paramMap.get('id');
  readonly isEditMode = !!this.saleId;

  constructor() {
    if (this.isEditMode && this.saleId) {
      this.loadSale(this.saleId);
    }
  }

  hasUnsavedChanges(): boolean {
    return this.saleForm?.isDirty ?? false;
  }

  onSubmit(value: SaleFormSubmitValue): void {
    this.submitting.set(true);

    if (this.isEditMode && this.saleId) {
      this.salesApi
        .update(this.saleId, {
          customerId: value.customerId,
          customerName: value.customerName,
          branchId: value.branchId,
          branchName: value.branchName,
          saleDate: value.saleDate,
          items: value.items.map((item) => ({
            id: item.id,
            productId: item.productId,
            productName: item.productName,
            quantity: item.quantity,
            unitPrice: item.unitPrice,
          })),
        })
        .subscribe({
          next: (updated) => {
            this.submitting.set(false);
            this.showSuccess('Sale updated successfully');
            void this.router.navigate(['/sales', updated.id]);
          },
          error: (error) => {
            this.submitting.set(false);
            this.showError(error);
          },
        });

      return;
    }

    this.salesApi
      .create({
        customerId: value.customerId,
        customerName: value.customerName,
        branchId: value.branchId,
        branchName: value.branchName,
        saleDate: value.saleDate,
        items: value.items.map((item) => ({
          productId: item.productId,
          productName: item.productName,
          quantity: item.quantity,
          unitPrice: item.unitPrice,
        })),
      })
      .subscribe({
        next: (created) => {
          this.submitting.set(false);
          this.showSuccess('Sale created successfully');
          void this.router.navigate(['/sales', created.id]);
        },
        error: (error) => {
          this.submitting.set(false);
          this.showError(error);
        },
      });
  }

  onCancel(): void {
    if (this.isEditMode && this.saleId) {
      void this.router.navigate(['/sales', this.saleId]);
      return;
    }

    void this.router.navigate(['/sales']);
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
