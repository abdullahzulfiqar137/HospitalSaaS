import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { PagedResponse } from '../../../shared/models/api.models';
import { AuditLog, AuditLogQuery } from '../models/audit-log.model';

@Injectable({ providedIn: 'root' })
export class AuditLogService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/audit-logs`;

  list(query: AuditLogQuery = {}): Observable<PagedResponse<AuditLog>> {
    let params = new HttpParams();
    Object.entries(query).forEach(([key, value]) => {
      if (value !== undefined && value !== '') params = params.set(key, String(value));
    });
    return this.http.get<PagedResponse<AuditLog>>(this.url, { params });
  }
}
