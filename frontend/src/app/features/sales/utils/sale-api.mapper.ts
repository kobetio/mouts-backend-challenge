import {
  ExternalReference,
  Sale,
  SaleItem,
  SaleItemStatus,
  SaleStatus,
} from '../models/sale.model';

function readString(source: Record<string, unknown>, ...keys: string[]): string {
  for (const key of keys) {
    const value = source[key];
    if (typeof value === 'string') {
      return value;
    }
  }

  return '';
}

function readNumber(source: Record<string, unknown>, ...keys: string[]): number {
  for (const key of keys) {
    const value = source[key];
    if (typeof value === 'number' && !Number.isNaN(value)) {
      return value;
    }
  }

  return 0;
}

function mapExternalReference(raw: unknown): ExternalReference {
  if (typeof raw !== 'object' || raw === null) {
    return { id: '', name: '' };
  }

  const source = raw as Record<string, unknown>;
  return {
    id: readString(source, 'id', 'Id'),
    name: readString(source, 'name', 'Name'),
  };
}

function mapSaleItem(raw: unknown): SaleItem {
  const source = (typeof raw === 'object' && raw !== null ? raw : {}) as Record<string, unknown>;

  return {
    id: readString(source, 'id', 'Id'),
    product: mapExternalReference(source['product'] ?? source['Product']),
    quantity: readNumber(source, 'quantity', 'Quantity'),
    unitPrice: readNumber(source, 'unitPrice', 'UnitPrice'),
    discountPercentage: readNumber(source, 'discountPercentage', 'DiscountPercentage'),
    totalAmount: readNumber(source, 'totalAmount', 'TotalAmount'),
    status: readNumber(source, 'status', 'Status') as SaleItemStatus,
    createdAt: readString(source, 'createdAt', 'CreatedAt'),
    updatedAt: (source['updatedAt'] ?? source['UpdatedAt'] ?? null) as string | null,
  };
}

/** Normalizes API payloads (camelCase or PascalCase) into the frontend Sale model. */
export function mapApiSale(raw: unknown): Sale {
  const source = (typeof raw === 'object' && raw !== null ? raw : {}) as Record<string, unknown>;
  const itemsRaw = source['items'] ?? source['Items'];
  const items = Array.isArray(itemsRaw) ? itemsRaw.map(mapSaleItem) : [];

  return {
    id: readString(source, 'id', 'Id'),
    saleNumber: readNumber(source, 'saleNumber', 'SaleNumber'),
    saleDate: readString(source, 'saleDate', 'SaleDate'),
    customer: mapExternalReference(source['customer'] ?? source['Customer']),
    branch: mapExternalReference(source['branch'] ?? source['Branch']),
    totalAmount: readNumber(source, 'totalAmount', 'TotalAmount'),
    status: readNumber(source, 'status', 'Status') as SaleStatus,
    items,
    createdAt: readString(source, 'createdAt', 'CreatedAt'),
    updatedAt: (source['updatedAt'] ?? source['UpdatedAt'] ?? null) as string | null,
  };
}

export function mapApiSaleList(raw: unknown): Sale[] {
  if (!Array.isArray(raw)) {
    return [];
  }

  return raw.map(mapApiSale);
}
