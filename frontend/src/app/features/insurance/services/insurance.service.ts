import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { PagedResponse } from '../../../shared/models/api.models';
import {
  CreateInsurancePolicyRequest,
  InsurancePolicy,
  InsuranceQuery,
} from '../models/insurance.model';

@Injectable({ providedIn: 'root' })
export class InsuranceService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/insurance`;

  list(query: InsuranceQuery = {}): Observable<PagedResponse<InsurancePolicy>> {
    let params = new HttpParams();
    Object.entries(query).forEach(([key, value]) => {
      if (value !== undefined && value !== '') params = params.set(key, String(value));
    });
    return this.http.get<PagedResponse<InsurancePolicy>>(this.url, { params });
  }

  createPolicy(request: CreateInsurancePolicyRequest): Observable<InsurancePolicy> {
    return this.http.post<InsurancePolicy>(this.url, request);
  }
}
