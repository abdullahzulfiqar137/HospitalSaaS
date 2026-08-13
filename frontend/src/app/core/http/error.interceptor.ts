import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { ApiError } from '../../shared/models/api.models';
import { NotificationService } from './notification.service';
export const errorInterceptor: HttpInterceptorFn = (request, next) => {
  const notices = inject(NotificationService);
  return next(request).pipe(
    catchError((error: HttpErrorResponse) => {
      // Ignore auth and 401 errors to prevent annoying toast popups
      if (request.url.includes('/auth/') || error.status === 401) {
        return throwError(() => error);
      }
      const body = error.error as ApiError | undefined;
      const fallback: Record<number, string> = {
        0: 'Cannot reach the hospital API.',
        401: 'Your session has expired.',
        403: 'You do not have permission for this action.',
        404: 'The requested record was not found.',
        500: 'The server could not complete the request.',
      };
      notices.show(body?.message ?? fallback[error.status] ?? 'Request failed.', 'error');
      return throwError(() => error);
    }),
  );
};
