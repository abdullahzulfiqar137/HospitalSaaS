import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/http/api.service';
import { CreateDoctorRequest, Doctor, UpdateDoctorRequest } from '../models/doctor.model';

@Injectable({ providedIn: 'root' })
export class DoctorService extends ApiService<Doctor> {
  protected readonly resource = 'doctors';

  createDoctor(request: CreateDoctorRequest): Observable<Doctor> {
    return this.create(request);
  }
  update(id: string, request: UpdateDoctorRequest): Observable<Doctor> {
    return this.http.put<Doctor>(`${this.url}/${id}`, request);
  }
  activate(id: string): Observable<void> {
    return this.http.post<void>(`${this.url}/${id}/activate`, {});
  }
  deactivate(id: string): Observable<void> {
    return this.http.post<void>(`${this.url}/${id}/deactivate`, {});
  }
}
