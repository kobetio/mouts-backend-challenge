export interface ApiErrorResponse {
  type: string;
  error: string;
  detail: string;
}

export interface ApiResponse<T = unknown> {
  success: boolean;
  message: string;
  data?: T;
  errors?: string[];
}

export interface PaginatedApiResponse<T> extends ApiResponse<T[]> {
  currentPage: number;
  totalPages: number;
  totalItems: number;
}

export class ApiClientError extends Error {
  constructor(
    message: string,
    readonly status: number,
    readonly body: ApiErrorResponse | null,
  ) {
    super(message);
    this.name = 'ApiClientError';
  }
}
