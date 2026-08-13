import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/http/api.service';
import { Patient } from '../models/patient.model';
import { CreatePatientRequest, UpdatePatientRequest } from '../models/patient-request.model';
@Injectable({ providedIn: 'root' })
export class PatientService extends ApiService<Patient> {
  protected readonly resource = 'patients';
  override create<R>(request: R): Observable<Patient> {
    return this.http.post<Patient>(this.url, request);
  }
  update(id: string, request: UpdatePatientRequest): Observable<Patient> {
    return this.http.put<Patient>(`${this.url}/${id}`, request);
  }
}
