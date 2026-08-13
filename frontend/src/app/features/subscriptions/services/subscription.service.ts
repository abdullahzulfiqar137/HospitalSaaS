import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { PageQuery, PagedResponse } from '../../../shared/models/api.models';
import { CreateSubscriptionRequest, Subscription } from '../models/subscription.model';
@Injectable({ providedIn: 'root' })
export class SubscriptionService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/subscriptions`;

  list(
    query: Pick<PageQuery, 'pageNumber' | 'pageSize'> = {},
  ): Observable<PagedResponse<Subscription>> {
    let params = new HttpParams();
    Object.entries(query).forEach(([key, value]) => {
      if (value !== undefined) params = params.set(key, String(value));
    });
    return this.http.get<PagedResponse<Subscription>>(this.url, { params });
  }

  create(request: CreateSubscriptionRequest): Observable<Subscription> {
    return this.http.post<Subscription>(this.url, request);
  }
}
