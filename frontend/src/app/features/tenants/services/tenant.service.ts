import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { CreateTenantRequest, Tenant } from '../models/tenant.model';
@Injectable({ providedIn: 'root' })
export class TenantService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/tenants`;

  create(request: CreateTenantRequest): Observable<Tenant> {
    return this.http.post<Tenant>(this.url, request);
  }
}
