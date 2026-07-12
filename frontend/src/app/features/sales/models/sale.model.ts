export enum SaleStatus {
  NotCancelled = 0,
  Cancelled = 1,
}

export enum SaleItemStatus {
  NotCancelled = 0,
  Cancelled = 1,
}

export interface ExternalReference {
  id: string;
  name: string;
}

export interface SaleItem {
  id: string;
  product: ExternalReference;
  quantity: number;
  unitPrice: number;
  discountPercentage: number;
  totalAmount: number;
  status: SaleItemStatus;
  createdAt: string;
  updatedAt: string | null;
}

export interface Sale {
  id: string;
  saleNumber: number;
  saleDate: string;
  customer: ExternalReference;
  branch: ExternalReference;
  totalAmount: number;
  status: SaleStatus;
  items: SaleItem[];
  createdAt: string;
  updatedAt: string | null;
}

export interface CreateSaleItemRequest {
  productId: string;
  productName: string;
  quantity: number;
  unitPrice: number;
}

export interface CreateSaleRequest {
  customerId: string;
  customerName: string;
  branchId: string;
  branchName: string;
  saleDate?: string;
  items: CreateSaleItemRequest[];
}

export interface UpdateSaleItemRequest {
  id?: string | null;
  productId: string;
  productName: string;
  quantity: number;
  unitPrice: number;
}

export interface UpdateSaleRequest {
  customerId: string;
  customerName: string;
  branchId: string;
  branchName: string;
  saleDate: string;
  items: UpdateSaleItemRequest[];
}

export interface SaleListQuery {
  page?: number;
  size?: number;
  orderBy?: string;
  customerName?: string;
  branchName?: string;
  cancelled?: boolean | null;
  customerId?: string;
  branchId?: string;
  minTotalAmount?: number | null;
  maxTotalAmount?: number | null;
  minDate?: string | null;
  maxDate?: string | null;
}

export interface SaleListResult {
  items: Sale[];
  currentPage: number;
  totalPages: number;
  totalItems: number;
}

export function isSaleCancelled(status: SaleStatus): boolean {
  return status === SaleStatus.Cancelled;
}

export function isSaleItemCancelled(status: SaleItemStatus): boolean {
  return status === SaleItemStatus.Cancelled;
}

export function saleStatusLabel(status: SaleStatus): string {
  return isSaleCancelled(status) ? 'Cancelled' : 'Active';
}

export function saleItemStatusLabel(status: SaleItemStatus): string {
  return isSaleItemCancelled(status) ? 'Cancelled' : 'Active';
}
