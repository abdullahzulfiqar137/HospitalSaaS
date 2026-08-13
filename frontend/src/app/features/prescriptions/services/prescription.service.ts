import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/http/api.service';
import { PagedResponse } from '../../../shared/models/api.models';
import { CreatePrescriptionRequest, Prescription } from '../models/prescription.model';
@Injectable({ providedIn: 'root' })
export class PrescriptionService extends ApiService<Prescription> {
  protected readonly resource = 'prescriptions';
  search(
    patientId?: string,
    pageNumber = 1,
    pageSize = 20,
  ): Observable<PagedResponse<Prescription>> {
    let params = new HttpParams().set('pageNumber', pageNumber).set('pageSize', pageSize);
    if (patientId) params = params.set('patientId', patientId);
    return this.http.get<PagedResponse<Prescription>>(this.url, { params });
  }
  createPrescription(request: CreatePrescriptionRequest): Observable<Prescription> {
    return this.create(request);
  }
}
