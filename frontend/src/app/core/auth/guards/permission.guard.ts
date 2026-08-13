import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
export const permissionGuard: CanActivateFn = (route) =>
  inject(AuthService).hasPermission(String(route.data?.['permission'])) ||
  inject(Router).createUrlTree(['/dashboard']);
