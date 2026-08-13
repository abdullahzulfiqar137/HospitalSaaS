import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/http/api.service';
import { PagedResponse } from '../../../shared/models/api.models';
import {
  Appointment,
  AppointmentActionRequest,
  AppointmentQuery,
  BookAppointmentRequest,
  UpdateAppointmentRequest,
} from '../models/appointment.model';

@Injectable({ providedIn: 'root' })
export class AppointmentService extends ApiService<Appointment> {
  protected readonly resource = 'appointments';

  search(query: AppointmentQuery = {}): Observable<PagedResponse<Appointment>> {
    let params = new HttpParams();
    for (const [key, value] of Object.entries(query))
      if (value !== undefined && value !== '') params = params.set(key, String(value));
    return this.http.get<PagedResponse<Appointment>>(this.url, { params });
  }
  book(request: BookAppointmentRequest): Observable<Appointment> {
    return this.create(request);
  }
  update(id: string, request: UpdateAppointmentRequest): Observable<Appointment> {
    return this.http.put<Appointment>(`${this.url}/${id}`, request);
  }
  action(id: string, request: AppointmentActionRequest): Observable<Appointment> {
    return this.http.post<Appointment>(`${this.url}/${id}/actions`, request);
  }
}
