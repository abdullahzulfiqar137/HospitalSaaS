import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/http/api.service';
import {
  AdjustInventoryRequest,
  CreateInventoryItemRequest,
  InventoryItem,
  InventoryStock,
  InventoryTransaction,
} from '../models/inventory.model';
@Injectable({ providedIn: 'root' })
export class InventoryService extends ApiService<InventoryItem> {
  protected readonly resource = 'inventory';

  createItem(request: CreateInventoryItemRequest): Observable<InventoryItem> {
    return this.create(request);
  }

  adjust(id: string, request: AdjustInventoryRequest): Observable<InventoryTransaction> {
    return this.http.post<InventoryTransaction>(`${this.url}/${id}/adjustments`, request);
  }

  stock(id: string): Observable<InventoryStock> {
    return this.http.get<InventoryStock>(`${this.url}/${id}/stock`);
  }
}
