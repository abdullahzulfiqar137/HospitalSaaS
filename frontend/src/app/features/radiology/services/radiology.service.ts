import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { PagedResponse } from '../../../shared/models/api.models';
import {
  CreateRadiologyOrderRequest,
  CreateRadiologyTestRequest,
  RadiologyOrder,
  RadiologyResult,
  RadiologyTest,
} from '../models/radiology.model';
@Injectable({ providedIn: 'root' })
export class RadiologyService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/radiology`;
  tests(search = '', pageNumber = 1, pageSize = 20): Observable<PagedResponse<RadiologyTest>> {
    return this.http.get<PagedResponse<RadiologyTest>>(`${this.url}/tests`, {
      params: { search, pageNumber, pageSize },
    });
  }
  createTest(request: CreateRadiologyTestRequest): Observable<RadiologyTest> {
    return this.http.post<RadiologyTest>(`${this.url}/tests`, request);
  }
  orders(
    patientId?: string,
    pageNumber = 1,
    pageSize = 20,
  ): Observable<PagedResponse<RadiologyOrder>> {
    let params = new HttpParams().set('pageNumber', pageNumber).set('pageSize', pageSize);
    if (patientId) params = params.set('patientId', patientId);
    return this.http.get<PagedResponse<RadiologyOrder>>(`${this.url}/orders`, { params });
  }
  getOrder(id: string): Observable<RadiologyOrder> {
    return this.http.get<RadiologyOrder>(`${this.url}/orders/${id}`);
  }
  createOrder(request: CreateRadiologyOrderRequest): Observable<RadiologyOrder> {
    return this.http.post<RadiologyOrder>(`${this.url}/orders`, request);
  }
  recordReport(id: string, report: string): Observable<RadiologyOrder> {
    return this.http.post<RadiologyOrder>(`${this.url}/orders/${id}/report`, { report });
  }
  resultHistory(id: string): Observable<RadiologyResult[]> {
    return this.http.get<RadiologyResult[]>(`${this.url}/orders/${id}/results/history`);
  }
}
