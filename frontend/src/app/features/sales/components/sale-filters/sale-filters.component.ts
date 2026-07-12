import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  Output,
  inject,
} from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { SaleListQuery } from '../../models/sale.model';

export interface SaleFiltersValue {
  customerName: string;
  branchName: string;
  cancelled: 'all' | 'true' | 'false';
  minTotalAmount: number | null;
  maxTotalAmount: number | null;
  minDate: string;
  maxDate: string;
}

@Component({
  selector: 'app-sale-filters',
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
  ],
  templateUrl: './sale-filters.component.html',
  styleUrl: './sale-filters.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SaleFiltersComponent {
  private readonly fb = inject(FormBuilder);

  @Input() set initialFilters(value: SaleListQuery | null) {
    if (!value) {
      return;
    }

    this.filtersForm.patchValue({
      customerName: value.customerName ?? '',
      branchName: value.branchName ?? '',
      cancelled:
        value.cancelled === true ? 'true' : value.cancelled === false ? 'false' : 'all',
      minTotalAmount: value.minTotalAmount ?? null,
      maxTotalAmount: value.maxTotalAmount ?? null,
      minDate: value.minDate ? value.minDate.substring(0, 10) : '',
      maxDate: value.maxDate ? value.maxDate.substring(0, 10) : '',
    });
  }

  @Output() readonly filtersApply = new EventEmitter<SaleListQuery>();
  @Output() readonly filtersReset = new EventEmitter<void>();

  readonly filtersForm = this.fb.group({
    customerName: this.fb.nonNullable.control(''),
    branchName: this.fb.nonNullable.control(''),
    cancelled: this.fb.nonNullable.control<'all' | 'true' | 'false'>('all'),
    minTotalAmount: this.fb.control<number | null>(null),
    maxTotalAmount: this.fb.control<number | null>(null),
    minDate: this.fb.nonNullable.control(''),
    maxDate: this.fb.nonNullable.control(''),
  });

  onApply(): void {
    const raw = this.filtersForm.getRawValue();
    const query: SaleListQuery = {};

    if (raw.customerName.trim()) {
      query.customerName = raw.customerName.trim();
    }

    if (raw.branchName.trim()) {
      query.branchName = raw.branchName.trim();
    }

    if (raw.cancelled !== 'all') {
      query.cancelled = raw.cancelled === 'true';
    }

    if (raw.minTotalAmount != null && raw.minTotalAmount !== ('' as unknown)) {
      query.minTotalAmount = Number(raw.minTotalAmount);
    }

    if (raw.maxTotalAmount != null && raw.maxTotalAmount !== ('' as unknown)) {
      query.maxTotalAmount = Number(raw.maxTotalAmount);
    }

    if (raw.minDate) {
      query.minDate = raw.minDate;
    }

    if (raw.maxDate) {
      query.maxDate = raw.maxDate;
    }

    this.filtersApply.emit(query);
  }

  onReset(): void {
    this.filtersForm.reset({
      customerName: '',
      branchName: '',
      cancelled: 'all',
      minTotalAmount: null,
      maxTotalAmount: null,
      minDate: '',
      maxDate: '',
    });
    this.filtersReset.emit();
  }
}
