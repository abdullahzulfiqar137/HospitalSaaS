import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { CurrentUser, LoginRequest, LoginResponse, SessionTokens } from '../models/auth.models';
import { TokenStorageService } from './token-storage.service';

const ROLE_PERMISSIONS_MAP: Record<string, string[]> = {
  admin: ['*'],
  'hospital admin': ['*'],
  doctor: [
    'Dashboard.View',
    'Patients.View',
    'Doctors.View',
    'Appointments.View',
    'Admissions.View',
    'MedicalRecords.View',
    'Prescriptions.View',
    'Laboratory.View',
    'Radiology.View',
    'Billing.View',
    'Payments.View',
  ],
  supplier: [
    'Dashboard.View',
    'Inventory.View',
    'Suppliers.View',
    'PurchaseOrders.View',
    'Billing.View',
    'Payments.View',
    'Patients.View',
  ],
  buyer: [
    'Dashboard.View',
    'Inventory.View',
    'PurchaseOrders.View',
    'Suppliers.View',
    'Billing.View',
    'Payments.View',
    'Patients.View',
  ],
  labtech: [
    'Dashboard.View',
    'Laboratory.View',
    'LabTests.View',
    'MedicalRecords.View',
    'Billing.View',
    'Payments.View',
    'Patients.View',
  ],
  pharmacist: [
    'Dashboard.View',
    'Pharmacy.View',
    'Inventory.View',
    'Prescriptions.View',
    'Billing.View',
    'Payments.View',
    'Patients.View',
  ],
  patient: [
    'Dashboard.View',
    'Appointments.View',
    'Prescriptions.View',
    'Laboratory.View',
    'Billing.View',
    'Payments.View',
    'Patients.View',
  ],
};

function base64UrlEncode(str: string): string {
  const base64 = btoa(
    encodeURIComponent(str).replace(/%([0-9A-F]{2})/g, (_, p1) =>
      String.fromCharCode(parseInt(p1, 16)),
    ),
  );
  return base64.replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly storage = inject(TokenStorageService);
  private readonly router = inject(Router);
  private readonly tokensState = signal<SessionTokens | null>(this.storage.get());
  readonly tokens = this.tokensState.asReadonly();
  readonly isAuthenticated = computed(() => {
    const tokens = this.tokensState();
    if (!tokens?.accessToken) return false;
    const expiresAt = Date.parse(tokens.expiresAtUtc);
    return Number.isFinite(expiresAt) && expiresAt > Date.now();
  });
  readonly user = computed<CurrentUser | null>(() => this.decode(this.tokensState()?.accessToken));

  login(request: LoginRequest, tenantId?: string): Observable<LoginResponse> {
    const headers = tenantId ? new HttpHeaders({ 'X-Tenant-ID': tenantId }) : undefined;
    return this.http
      .post<LoginResponse>(`${environment.apiUrl}/auth/login`, request, {
        headers,
        withCredentials: true,
      })
      .pipe(tap((tokens) => this.save(tokens)));
  }

  mockLogin(email: string, roleName: string): void {
    const header = base64UrlEncode(JSON.stringify({ alg: 'HS256', typ: 'JWT' }));
    const payload = base64UrlEncode(
      JSON.stringify({
        sub: '00000000-0000-0000-0000-000000000099',
        tenant_id: '00000000-0000-0000-0000-000000000001',
        email,
        role: [roleName],
        permission: ROLE_PERMISSIONS_MAP[roleName.toLowerCase()] ?? ['*'],
      }),
    );
    const mockAccessToken = `${header}.${payload}.signature`;
    const mockExpiresAt = new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString();
    this.save({
      accessToken: mockAccessToken,
      refreshToken: mockAccessToken,
      expiresAtUtc: mockExpiresAt,
    });
  }

  refresh(): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${environment.apiUrl}/auth/refresh`, null, { withCredentials: true })
      .pipe(tap((tokens) => this.save(tokens)));
  }

  logout(): void {
    this.http
      .post<void>(`${environment.apiUrl}/auth/logout`, null, { withCredentials: true })
      .subscribe({ error: () => undefined });
    this.clear();
  }

  hasPermission(permission: string): boolean {
    const u = this.user();
    if (!u) return false;
    const permissions = u.permissions ?? [];
    if (permissions.includes('*') || permissions.includes(permission)) return true;

    // Check role map fallbacks
    const roles = u.roles.map((r) => r.toLowerCase());
    for (const role of roles) {
      const allowed = ROLE_PERMISSIONS_MAP[role];
      if (allowed && (allowed.includes('*') || allowed.includes(permission))) {
        return true;
      }
    }
    return false;
  }

  clear(): void {
    this.storage.clear();
    this.tokensState.set(null);
    void this.router.navigate(['/login']);
  }

  private save(tokens: LoginResponse): void {
    const session = { accessToken: tokens.accessToken, expiresAtUtc: tokens.expiresAtUtc };
    this.storage.set(session);
    this.tokensState.set(session);
  }

  private decode(token?: string): CurrentUser | null {
    if (!token) return null;
    try {
      const parts = token.split('.');
      if (parts.length < 2) return null;
      let base64 = parts[1].replace(/-/g, '+').replace(/_/g, '/');
      while (base64.length % 4) {
        base64 += '=';
      }
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split('')
          .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
          .join(''),
      );
      const payload = JSON.parse(jsonPayload) as Record<string, unknown>;
      const list = (v: unknown) => (Array.isArray(v) ? v.map(String) : v ? [String(v)] : []);
      const emailStr = String(payload['email'] ?? payload['sub'] ?? '');
      const roles = list(payload['role']);
      let inferredPermissions = list(payload['permission']);

      // Infer role from email if missing in token payload
      if (roles.length === 0) {
        if (emailStr.includes('doctor')) roles.push('doctor');
        else if (emailStr.includes('supplier')) roles.push('supplier');
        else if (emailStr.includes('buyer')) roles.push('buyer');
        else if (emailStr.includes('labtech')) roles.push('labtech');
        else if (emailStr.includes('pharmacist')) roles.push('pharmacist');
        else if (emailStr.includes('patient') || emailStr.includes('vance')) roles.push('patient');
        else roles.push('admin');
      }

      if (inferredPermissions.length === 0) {
        const firstRole = roles[0]?.toLowerCase();
        inferredPermissions = ROLE_PERMISSIONS_MAP[firstRole] ?? ['*'];
      }

      return {
        userId: String(payload['sub'] ?? ''),
        tenantId: String(payload['tenant_id'] ?? ''),
        email: emailStr,
        permissions: inferredPermissions,
        roles: roles,
      };
    } catch {
      return null;
    }
  }
}
