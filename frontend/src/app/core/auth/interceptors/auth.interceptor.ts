import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const auth = inject(AuthService);
  const token = auth.tokens()?.accessToken;
  const tenantId = auth.user()?.tenantId;
  const effectiveTenantId = tenantId || '00000000-0000-0000-0000-000000000001';

  const headersToAdd: Record<string, string> = {};
  if (token && request.url.startsWith(environment.apiUrl)) {
    headersToAdd['Authorization'] = `Bearer ${token}`;
  }
  if (!request.headers.has('X-Tenant-ID') && request.url.startsWith(environment.apiUrl)) {
    headersToAdd['X-Tenant-ID'] = effectiveTenantId;
  }

  const outgoing =
    Object.keys(headersToAdd).length > 0
      ? request.clone({ setHeaders: headersToAdd })
      : request;

  return next(outgoing).pipe(
    catchError((error) => {
      if (
        error instanceof HttpErrorResponse &&
        error.status === 401 &&
        !request.url.includes('/auth/')
      )
        return auth.refresh().pipe(
          switchMap((tokens) =>
            next(
              request.clone({
                setHeaders: { Authorization: `Bearer ${tokens.accessToken}` },
              }),
            ),
          ),
          catchError((refreshError) => {
            return throwError(() => refreshError);
          }),
        );
      return throwError(() => error);
    }),
  );
};
