import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { PageQuery, PagedResponse } from '../../../shared/models/api.models';
import { CreateStaffRequest, Staff } from '../models/staff.model';
@Injectable({ providedIn: 'root' })
export class StaffService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/staff`;

  list(query: PageQuery = {}): Observable<PagedResponse<Staff>> {
    let params = new HttpParams();
    Object.entries(query).forEach(([key, value]) => {
      if (value !== undefined && value !== '') params = params.set(key, String(value));
    });
    return this.http.get<PagedResponse<Staff>>(this.url, { params });
  }

  create(request: CreateStaffRequest): Observable<Staff> {
    return this.http.post<Staff>(this.url, request);
  }
}
