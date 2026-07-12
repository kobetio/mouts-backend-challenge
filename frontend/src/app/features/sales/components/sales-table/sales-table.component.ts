import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  Output,
  ViewChild,
} from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSort, MatSortModule, Sort } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { RouterLink } from '@angular/router';
import {
  Sale,
  SaleStatus,
  isSaleCancelled,
  saleStatusLabel,
} from '../../models/sale.model';

@Component({
  selector: 'app-sales-table',
  imports: [
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    RouterLink,
  ],
  templateUrl: './sales-table.component.html',
  styleUrl: './sales-table.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SalesTableComponent {
  @ViewChild(MatSort) sort?: MatSort;

  @Input({ required: true }) sales: Sale[] = [];
  @Input({ required: true }) totalItems = 0;
  @Input({ required: true }) pageIndex = 0;
  @Input({ required: true }) pageSize = 10;
  @Input() sortActive = 'saleDate';
  @Input() sortDirection: 'asc' | 'desc' = 'desc';

  @Output() readonly pageChange = new EventEmitter<PageEvent>();
  @Output() readonly sortChange = new EventEmitter<Sort>();
  @Output() readonly viewSale = new EventEmitter<string>();
  @Output() readonly editSale = new EventEmitter<string>();
  @Output() readonly deleteSale = new EventEmitter<Sale>();
  @Output() readonly cancelSale = new EventEmitter<Sale>();

  readonly displayedColumns = [
    'saleNumber',
    'saleDate',
    'customer',
    'branch',
    'totalAmount',
    'status',
    'actions',
  ];

  readonly SaleStatus = SaleStatus;

  get pageSizeOptions(): number[] {
    return [5, 10, 25, 50];
  }

  statusLabel(status: SaleStatus): string {
    return saleStatusLabel(status);
  }

  isCancelled(status: SaleStatus): boolean {
    return isSaleCancelled(status);
  }

  formatDate(value: string): string {
    return new Date(value).toLocaleString();
  }

  formatCurrency(value: number): string {
    return value.toLocaleString(undefined, { style: 'currency', currency: 'USD' });
  }

  onSortChange(sort: Sort): void {
    this.sortChange.emit(sort);
  }

  onPageChange(event: PageEvent): void {
    this.pageChange.emit(event);
  }
}
