import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/http/api.service';
import { PagedResponse } from '../../../shared/models/api.models';
import {
  CreateMedicalRecordRequest,
  DiagnosisRequest,
  MedicalRecord,
} from '../models/medical-record.model';
@Injectable({ providedIn: 'root' })
export class MedicalRecordService extends ApiService<MedicalRecord> {
  protected readonly resource = 'medical-records';
  search(
    patientId?: string,
    pageNumber = 1,
    pageSize = 20,
  ): Observable<PagedResponse<MedicalRecord>> {
    let params = new HttpParams().set('pageNumber', pageNumber).set('pageSize', pageSize);
    if (patientId) params = params.set('patientId', patientId);
    return this.http.get<PagedResponse<MedicalRecord>>(this.url, { params });
  }
  createRecord(request: CreateMedicalRecordRequest): Observable<MedicalRecord> {
    return this.create(request);
  }
  addDiagnosis(id: string, request: DiagnosisRequest): Observable<MedicalRecord> {
    return this.http.post<MedicalRecord>(`${this.url}/${id}/diagnoses`, request);
  }
}
