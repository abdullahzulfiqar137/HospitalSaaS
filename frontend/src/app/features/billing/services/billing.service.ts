import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { PagedResponse } from '../../../shared/models/api.models';
import {
  CreateInvoiceRequest,
  Invoice,
  InvoiceQuery,
  Payment,
  RecordPaymentRequest,
} from '../models/billing.model';
@Injectable({ providedIn: 'root' })
export class BillingService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/billing/invoices`;

  list(query: InvoiceQuery = {}): Observable<PagedResponse<Invoice>> {
    return this.http.get<PagedResponse<Invoice>>(this.url, { params: this.params(query) });
  }

  get(id: string): Observable<Invoice> {
    return this.http.get<Invoice>(`${this.url}/${id}`);
  }

  create(request: CreateInvoiceRequest): Observable<Invoice> {
    return this.http.post<Invoice>(this.url, request);
  }

  recordPayment(invoiceId: string, request: RecordPaymentRequest): Observable<Payment> {
    return this.http.post<Payment>(`${this.url}/${invoiceId}/payments`, request);
  }

  private params(query: InvoiceQuery): HttpParams {
    let params = new HttpParams();
    Object.entries(query).forEach(([key, value]) => {
      if (value !== undefined && value !== '') params = params.set(key, String(value));
    });
    return params;
  }
}
