import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import type { ApiError } from './error.types';

export const errorInterceptor: HttpInterceptorFn = (req, next) =>
    next(req).pipe(
        catchError((err: unknown) => {
            if (err instanceof HttpErrorResponse) {
                const apiErr: ApiError = {
                    status: err.status,
                    message: (err.error?.message ?? err.message ?? 'Request failed'),
                    details: err.error
                };
                return throwError(() => apiErr);
            }
            return throwError(() => err);
        })
    );
