import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  Admission,
  AdmissionHistory,
  AdmitPatientRequest,
  TransferAdmissionRequest,
} from '../models/admission.model';

@Injectable({ providedIn: 'root' })
export class AdmissionService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/admissions`;

  get(id: string): Observable<Admission> {
    return this.http.get<Admission>(`${this.url}/${id}`);
  }

  history(id: string): Observable<AdmissionHistory[]> {
    return this.http.get<AdmissionHistory[]>(`${this.url}/${id}/history`);
  }

  admit(request: AdmitPatientRequest): Observable<Admission> {
    return this.http.post<Admission>(this.url, request);
  }

  transfer(id: string, request: TransferAdmissionRequest): Observable<Admission> {
    return this.http.post<Admission>(`${this.url}/${id}/transfer`, request);
  }

  discharge(id: string): Observable<void> {
    return this.http.post<void>(`${this.url}/${id}/discharge`, {});
  }
}
