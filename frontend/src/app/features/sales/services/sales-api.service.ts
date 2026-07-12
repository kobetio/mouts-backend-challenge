import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { map, Observable } from 'rxjs';
import {
  ApiResponse,
  PaginatedApiResponse,
} from '../../../core/models/api-response.model';
import {
  CreateSaleRequest,
  Sale,
  SaleListQuery,
  SaleListResult,
  UpdateSaleRequest,
} from '../models/sale.model';

@Injectable({ providedIn: 'root' })
export class SalesApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/sales';

  list(query: SaleListQuery): Observable<SaleListResult> {
    return this.http
      .get<PaginatedApiResponse<Sale>>(this.baseUrl, { params: this.buildListParams(query) })
      .pipe(
        map((response) => ({
          items: response.data ?? [],
          currentPage: response.currentPage,
          totalPages: response.totalPages,
          totalItems: response.totalItems,
        })),
      );
  }

  getById(id: string): Observable<Sale> {
    return this.http
      .get<ApiResponse<Sale>>(`${this.baseUrl}/${id}`)
      .pipe(map((response) => this.unwrapData(response)));
  }

  create(request: CreateSaleRequest): Observable<Sale> {
    return this.http
      .post<ApiResponse<Sale>>(this.baseUrl, request)
      .pipe(map((response) => this.unwrapData(response)));
  }

  update(id: string, request: UpdateSaleRequest): Observable<Sale> {
    return this.http
      .put<ApiResponse<Sale>>(`${this.baseUrl}/${id}`, request)
      .pipe(map((response) => this.unwrapData(response)));
  }

  delete(id: string): Observable<void> {
    return this.http.delete<ApiResponse>(`${this.baseUrl}/${id}`).pipe(map(() => undefined));
  }

  cancelSale(id: string): Observable<Sale> {
    return this.http
      .post<ApiResponse<Sale>>(`${this.baseUrl}/${id}/cancel`, {})
      .pipe(map((response) => this.unwrapData(response)));
  }

  cancelItem(saleId: string, itemId: string): Observable<Sale> {
    return this.http
      .post<ApiResponse<Sale>>(`${this.baseUrl}/${saleId}/items/${itemId}/cancel`, {})
      .pipe(map((response) => this.unwrapData(response)));
  }

  private unwrapData<T>(response: ApiResponse<T>): T {
    if (response.data === undefined || response.data === null) {
      throw new Error('API response did not include data.');
    }

    return response.data;
  }

  private buildListParams(query: SaleListQuery): HttpParams {
    let params = new HttpParams();

    if (query.page != null) {
      params = params.set('_page', String(query.page));
    }

    if (query.size != null) {
      params = params.set('_size', String(query.size));
    }

    if (query.orderBy) {
      params = params.set('_order', query.orderBy);
    }

    if (query.customerName) {
      params = params.set('customerName', query.customerName);
    }

    if (query.branchName) {
      params = params.set('branchName', query.branchName);
    }

    if (query.cancelled != null) {
      params = params.set('cancelled', String(query.cancelled));
    }

    if (query.customerId) {
      params = params.set('customerId', query.customerId);
    }

    if (query.branchId) {
      params = params.set('branchId', query.branchId);
    }

    if (query.minTotalAmount != null) {
      params = params.set('_minTotalAmount', String(query.minTotalAmount));
    }

    if (query.maxTotalAmount != null) {
      params = params.set('_maxTotalAmount', String(query.maxTotalAmount));
    }

    if (query.minDate) {
      params = params.set('_minDate', query.minDate);
    }

    if (query.maxDate) {
      params = params.set('_maxDate', query.maxDate);
    }

    return params;
  }
}
