import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { PageQuery, PagedResponse } from '../../../shared/models/api.models';
import { CreateRoleRequest, Role } from '../models/role.model';
@Injectable({ providedIn: 'root' })
export class RoleService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/roles`;

  list(query: Pick<PageQuery, 'pageNumber' | 'pageSize'> = {}): Observable<PagedResponse<Role>> {
    let params = new HttpParams();
    Object.entries(query).forEach(([key, value]) => {
      if (value !== undefined) params = params.set(key, String(value));
    });
    return this.http.get<PagedResponse<Role>>(this.url, { params });
  }

  create(request: CreateRoleRequest): Observable<Role> {
    return this.http.post<Role>(this.url, request);
  }
}
