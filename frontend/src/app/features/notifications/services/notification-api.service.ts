import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { PagedResponse } from '../../../shared/models/api.models';
import {
  CreateNotificationRequest,
  Notification,
  NotificationQuery,
} from '../models/notification.model';
@Injectable({ providedIn: 'root' })
export class NotificationApiService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/notifications`;

  list(query: NotificationQuery = {}): Observable<PagedResponse<Notification>> {
    let params = new HttpParams();
    Object.entries(query).forEach(([key, value]) => {
      if (value !== undefined && value !== '') params = params.set(key, String(value));
    });
    return this.http.get<PagedResponse<Notification>>(this.url, { params });
  }

  create(request: CreateNotificationRequest): Observable<Notification> {
    return this.http.post<Notification>(this.url, request);
  }
}
