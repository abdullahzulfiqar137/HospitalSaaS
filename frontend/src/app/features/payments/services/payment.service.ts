import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { PagedResponse } from '../../../shared/models/api.models';
import { Payment, PaymentQuery, RefundPaymentRequest } from '../../billing/models/billing.model';
@Injectable({ providedIn: 'root' })
export class PaymentService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/billing/payments`;

  list(query: PaymentQuery = {}): Observable<PagedResponse<Payment>> {
    let params = new HttpParams();
    Object.entries(query).forEach(([key, value]) => {
      if (value !== undefined && value !== '') params = params.set(key, String(value));
    });
    return this.http.get<PagedResponse<Payment>>(this.url, { params });
  }

  refund(id: string, request: RefundPaymentRequest): Observable<Payment> {
    return this.http.post<Payment>(`${this.url}/${id}/refund`, request);
  }
}
