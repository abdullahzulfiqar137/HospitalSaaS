import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { PagedResponse } from '../../../shared/models/api.models';
import {
  CreateLabOrderRequest,
  CreateLabTestRequest,
  LabOrder,
  LabResult,
  LabTest,
  RecordLabResultRequest,
} from '../models/laboratory.model';
@Injectable({ providedIn: 'root' })
export class LaboratoryService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/laboratory`;
  tests(search = '', pageNumber = 1, pageSize = 20): Observable<PagedResponse<LabTest>> {
    return this.http.get<PagedResponse<LabTest>>(`${this.url}/tests`, {
      params: { search, pageNumber, pageSize },
    });
  }
  createTest(request: CreateLabTestRequest): Observable<LabTest> {
    return this.http.post<LabTest>(`${this.url}/tests`, request);
  }
  orders(
    patientId?: string,
    status?: string,
    pageNumber = 1,
    pageSize = 20,
  ): Observable<PagedResponse<LabOrder>> {
    let params = new HttpParams().set('pageNumber', pageNumber).set('pageSize', pageSize);
    if (patientId) params = params.set('patientId', patientId);
    if (status) params = params.set('status', status);
    return this.http.get<PagedResponse<LabOrder>>(`${this.url}/orders`, { params });
  }
  getOrder(id: string): Observable<LabOrder> {
    return this.http.get<LabOrder>(`${this.url}/orders/${id}`);
  }
  createOrder(request: CreateLabOrderRequest): Observable<LabOrder> {
    return this.http.post<LabOrder>(`${this.url}/orders`, request);
  }
  action(id: string, action: string): Observable<LabOrder> {
    return this.http.post<LabOrder>(`${this.url}/orders/${id}/actions`, { action });
  }
  recordResult(id: string, request: RecordLabResultRequest): Observable<LabOrder> {
    return this.http.post<LabOrder>(`${this.url}/orders/${id}/results`, request);
  }
  resultHistory(id: string): Observable<LabResult[]> {
    return this.http.get<LabResult[]>(`${this.url}/orders/${id}/results/history`);
  }
}
