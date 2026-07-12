import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, finalize, throwError } from 'rxjs';
import { ApiClientError } from '../models/api-response.model';
import { LoadingService } from '../services/loading.service';
import { extractApiErrorMessage, toApiErrorResponse } from '../utils/api-error.util';

export const apiInterceptor: HttpInterceptorFn = (req, next) => {
  const loading = inject(LoadingService);

  loading.show();

  return next(req).pipe(
    finalize(() => loading.hide()),
    catchError((error: unknown) => {
      if (error instanceof HttpErrorResponse) {
        const message = extractApiErrorMessage(error);
        const body = toApiErrorResponse(error);
        return throwError(() => new ApiClientError(message, error.status, body));
      }

      return throwError(() => error);
    }),
  );
};
