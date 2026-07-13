import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  effect,
  inject,
  input,
  output,
  computed,
  signal,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { Sale } from '../../models/sale.model';
import {
  formatDiscountPercentage,
  previewLineDiscount,
  roundCurrency,
} from '../../utils/discount-policy';

export type SaleItemFormGroup = FormGroup<{
  id: FormControl<string | null>;
  productId: FormControl<string>;
  productName: FormControl<string>;
  quantity: FormControl<number>;
  unitPrice: FormControl<number>;
}>;

export type SaleFormGroup = FormGroup<{
  customerId: FormControl<string>;
  customerName: FormControl<string>;
  branchId: FormControl<string>;
  branchName: FormControl<string>;
  saleDate: FormControl<string>;
  items: FormArray<SaleItemFormGroup>;
}>;

export interface SaleFormSubmitValue {
  customerId: string;
  customerName: string;
  branchId: string;
  branchName: string;
  saleDate: string;
  items: {
    id?: string | null;
    productId: string;
    productName: string;
    quantity: number;
    unitPrice: number;
  }[];
}

@Component({
  selector: 'app-sale-form',
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
  ],
  templateUrl: './sale-form.component.html',
  styleUrl: './sale-form.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SaleFormComponent {
  private readonly fb = inject(FormBuilder);
  private readonly cdr = inject(ChangeDetectorRef);

  readonly sale = input<Sale | null>(null);
  readonly isEditMode = input(false);
  readonly submitting = input(false);

  readonly formSubmit = output<SaleFormSubmitValue>();
  readonly formCancel = output<void>();

  readonly form: SaleFormGroup = this.fb.group({
    customerId: this.fb.nonNullable.control('', [Validators.required]),
    customerName: this.fb.nonNullable.control('', [Validators.required]),
    branchId: this.fb.nonNullable.control('', [Validators.required]),
    branchName: this.fb.nonNullable.control('', [Validators.required]),
    saleDate: this.fb.nonNullable.control(this.todayIsoDate(), [Validators.required]),
    items: this.fb.array<SaleItemFormGroup>([]),
  });

  private readonly formRevision = signal(0);
  private readonly patchedSaleId = signal<string | null>(null);

  readonly linePreviews = computed(() => {
    this.formRevision();
    return this.items.controls.map((group) => {
      const quantity = Number(group.controls.quantity.value) || 0;
      const unitPrice = Number(group.controls.unitPrice.value) || 0;
      return previewLineDiscount(quantity, unitPrice);
    });
  });

  readonly saleTotal = computed(() => {
    const previews = this.linePreviews();
    const hasErrors = previews.some((preview) => preview.error);
    if (hasErrors) {
      return 0;
    }

    return roundCurrency(previews.reduce((sum, preview) => sum + preview.lineTotal, 0));
  });

  readonly hasQuantityErrors = computed(() =>
    this.linePreviews().some((preview) => !!preview.error),
  );

  constructor() {
    this.form.valueChanges.pipe(takeUntilDestroyed()).subscribe(() => {
      this.formRevision.update((value) => value + 1);
    });

    effect(() => {
      const sale = this.sale();
      if (sale) {
        this.applySaleToForm(sale);
        return;
      }

      if (!this.isEditMode() && this.items.length === 0) {
        this.resetCreateForm();
      }
    });
  }

  get items(): FormArray<SaleItemFormGroup> {
    return this.form.controls.items;
  }

  get pageTitle(): string {
    return this.isEditMode() ? 'Edit sale' : 'Create sale';
  }

  get submitLabel(): string {
    return this.isEditMode() ? 'Save changes' : 'Create sale';
  }

  get isDirty(): boolean {
    return this.form.dirty;
  }

  formatDiscount(fraction: number): string {
    return formatDiscountPercentage(fraction);
  }

  formatCurrency(value: number): string {
    return value.toLocaleString(undefined, { style: 'currency', currency: 'USD' });
  }

  addItem(): void {
    this.items.push(this.createItemGroup());
    this.form.markAsDirty();
    this.bumpFormRevision();
  }

  removeItem(index: number): void {
    if (this.items.length <= 1) {
      return;
    }

    this.items.removeAt(index);
    this.form.markAsDirty();
    this.bumpFormRevision();
  }

  onSubmit(): void {
    if (this.form.invalid || this.hasQuantityErrors()) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    this.formSubmit.emit({
      customerId: raw.customerId,
      customerName: raw.customerName,
      branchId: raw.branchId,
      branchName: raw.branchName,
      saleDate: raw.saleDate,
      items: raw.items.map((item) => ({
        id: item.id,
        productId: item.productId,
        productName: item.productName,
        quantity: Number(item.quantity),
        unitPrice: Number(item.unitPrice),
      })),
    });
  }

  onCancel(): void {
    this.formCancel.emit();
  }

  trackItemRow(index: number, group: SaleItemFormGroup): string {
    return group.controls.id.value ?? `row-${index}`;
  }

  private applySaleToForm(sale: Sale): void {
    if (this.patchedSaleId() === sale.id && this.items.length > 0) {
      return;
    }

    const itemGroups = (sale.items ?? []).map((item) =>
      this.createItemGroup({
        id: item.id,
        productId: item.product.id,
        productName: item.product.name,
        quantity: item.quantity,
        unitPrice: item.unitPrice,
      }),
    );

    if (itemGroups.length === 0) {
      itemGroups.push(this.createItemGroup());
    }

    this.form.setControl('items', this.fb.array(itemGroups));

    this.form.patchValue({
      customerId: sale.customer.id,
      customerName: sale.customer.name,
      branchId: sale.branch.id,
      branchName: sale.branch.name,
      saleDate: sale.saleDate.substring(0, 10),
    });

    this.form.markAsPristine();
    this.patchedSaleId.set(sale.id);
    this.bumpFormRevision();
    this.cdr.detectChanges();
  }

  private resetCreateForm(): void {
    this.form.setControl('items', this.fb.array([this.createItemGroup()]));
    this.form.patchValue({
      customerId: '',
      customerName: '',
      branchId: '',
      branchName: '',
      saleDate: this.todayIsoDate(),
    });
    this.form.markAsPristine();
    this.patchedSaleId.set(null);
    this.bumpFormRevision();
  }

  private createItemGroup(initial?: {
    id?: string | null;
    productId?: string;
    productName?: string;
    quantity?: number;
    unitPrice?: number;
  }): SaleItemFormGroup {
    return this.fb.group({
      id: this.fb.control<string | null>(initial?.id ?? null),
      productId: this.fb.nonNullable.control(initial?.productId ?? '', [Validators.required]),
      productName: this.fb.nonNullable.control(initial?.productName ?? '', [Validators.required]),
      quantity: this.fb.nonNullable.control(initial?.quantity ?? 1, [
        Validators.required,
        Validators.min(1),
        Validators.max(20),
      ]),
      unitPrice: this.fb.nonNullable.control(initial?.unitPrice ?? 0, [
        Validators.required,
        Validators.min(0.01),
      ]),
    });
  }

  private bumpFormRevision(): void {
    this.formRevision.update((value) => value + 1);
  }

  private todayIsoDate(): string {
    return new Date().toISOString().substring(0, 10);
  }
}
