import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import {
  SaleItem,
  SaleItemStatus,
  isSaleItemCancelled,
  saleItemStatusLabel,
} from '../../models/sale.model';
import { formatDiscountPercentage } from '../../utils/discount-policy';

@Component({
  selector: 'app-sale-items-table',
  imports: [MatTableModule, MatButtonModule, MatIconModule, MatChipsModule],
  templateUrl: './sale-items-table.component.html',
  styleUrl: './sale-items-table.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SaleItemsTableComponent {
  @Input({ required: true }) items: SaleItem[] = [];
  @Input() saleCancelled = false;
  @Input() showActions = true;

  @Output() readonly cancelItem = new EventEmitter<SaleItem>();

  readonly displayedColumns = [
    'product',
    'quantity',
    'unitPrice',
    'discount',
    'totalAmount',
    'status',
    'actions',
  ];

  get columns(): string[] {
    return this.showActions
      ? this.displayedColumns
      : this.displayedColumns.filter((column) => column !== 'actions');
  }

  statusLabel(status: SaleItemStatus): string {
    return saleItemStatusLabel(status);
  }

  isCancelled(status: SaleItemStatus): boolean {
    return isSaleItemCancelled(status);
  }

  formatDiscount(fraction: number): string {
    return formatDiscountPercentage(fraction);
  }

  formatCurrency(value: number): string {
    return value.toLocaleString(undefined, { style: 'currency', currency: 'USD' });
  }

  canCancelItem(item: SaleItem): boolean {
    return this.showActions && !this.saleCancelled && !this.isCancelled(item.status);
  }
}
