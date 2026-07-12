export interface DiscountPreview {
  discountPercentage: number;
  lineTotal: number;
  error?: string;
}

const NO_DISCOUNT_MAX = 3;
const LOW_TIER_MAX = 9;
const HIGH_TIER_MAX = 20;
const LOW_TIER_DISCOUNT = 0.1;
const HIGH_TIER_DISCOUNT = 0.2;

/** Mirrors backend TieredDiscountPolicy for client-side previews. */
export function getDiscountPercentage(quantity: number): number {
  if (quantity > HIGH_TIER_MAX) {
    throw new Error(
      `Cannot sell more than ${HIGH_TIER_MAX} identical items of the same product in a single sale.`,
    );
  }

  if (quantity > LOW_TIER_MAX) {
    return HIGH_TIER_DISCOUNT;
  }

  if (quantity > NO_DISCOUNT_MAX) {
    return LOW_TIER_DISCOUNT;
  }

  return 0;
}

export function computeLineTotal(quantity: number, unitPrice: number): number {
  if (quantity <= 0 || unitPrice < 0) {
    return 0;
  }

  try {
    const discount = getDiscountPercentage(quantity);
    return roundCurrency(quantity * unitPrice * (1 - discount));
  } catch {
    return 0;
  }
}

export function previewLineDiscount(quantity: number, unitPrice: number): DiscountPreview {
  if (quantity <= 0) {
    return { discountPercentage: 0, lineTotal: 0 };
  }

  if (quantity > HIGH_TIER_MAX) {
    return {
      discountPercentage: 0,
      lineTotal: 0,
      error: `Cannot sell more than ${HIGH_TIER_MAX} identical items of the same product in a single sale.`,
    };
  }

  const discountPercentage = getDiscountPercentage(quantity);
  const lineTotal = roundCurrency(quantity * unitPrice * (1 - discountPercentage));

  return { discountPercentage, lineTotal };
}

export function formatDiscountPercentage(fraction: number): string {
  return `${(fraction * 100).toFixed(0)}%`;
}

export function roundCurrency(value: number): number {
  return Math.round(value * 100) / 100;
}
