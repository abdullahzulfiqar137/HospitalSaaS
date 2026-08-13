import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { PagedResponse } from '../../../shared/models/api.models';
import {
  CreatePurchaseOrder,
  PurchaseOrder,
  PurchaseOrderAction,
  PurchaseOrderQuery,
} from '../models/purchase-order.model';
@Injectable({ providedIn: 'root' })
export class PurchaseOrderService {
  private http = inject(HttpClient);
  private url = `${environment.apiUrl}/inventory/purchase-orders`;
  list(query: PurchaseOrderQuery = {}) {
    let params = new HttpParams();
    Object.entries(query).forEach(([key, value]) => {
      if (value !== undefined && value !== '') params = params.set(key, String(value));
    });
    return this.http.get<PagedResponse<PurchaseOrder>>(this.url, { params });
  }
  get(id: string) {
    return this.http.get<PurchaseOrder>(`${this.url}/${id}`);
  }
  create(body: CreatePurchaseOrder) {
    return this.http.post<PurchaseOrder>(this.url, body);
  }
  action(id: string, action: PurchaseOrderAction) {
    return this.http.post<PurchaseOrder>(`${this.url}/${id}/actions`, { action });
  }
}
