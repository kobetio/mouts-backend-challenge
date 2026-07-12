import { HttpErrorResponse } from '@angular/common/http';
import { ApiErrorResponse } from '../models/api-response.model';

export function isApiErrorResponse(value: unknown): value is ApiErrorResponse {
  if (typeof value !== 'object' || value === null) {
    return false;
  }

  const candidate = value as Record<string, unknown>;
  return (
    typeof candidate['type'] === 'string' &&
    typeof candidate['error'] === 'string' &&
    typeof candidate['detail'] === 'string'
  );
}

/** Returns the backend error message verbatim when available. */
export function extractApiErrorMessage(error: HttpErrorResponse): string {
  const body = error.error;

  if (isApiErrorResponse(body)) {
    return body.detail;
  }

  if (typeof body === 'string' && body.length > 0) {
    return body;
  }

  return error.message || 'An unexpected error occurred.';
}

export function toApiErrorResponse(error: HttpErrorResponse): ApiErrorResponse | null {
  return isApiErrorResponse(error.error) ? error.error : null;
}
