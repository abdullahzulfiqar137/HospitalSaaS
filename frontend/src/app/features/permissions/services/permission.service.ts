import { Injectable, inject } from '@angular/core';
import { AuthService } from '../../../core/auth/services/auth.service';
@Injectable({ providedIn: 'root' })
export class PermissionService {
  private readonly auth = inject(AuthService);
  all(): readonly string[] {
    return this.auth.user()?.permissions ?? [];
  }
  has(value: string): boolean {
    return this.auth.hasPermission(value);
  }
}
