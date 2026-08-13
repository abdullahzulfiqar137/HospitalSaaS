import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/http/api.service';
import {
  CreateMedicineRequest,
  Medicine,
  StockBalance,
  StockRequest,
  StockTransaction,
} from '../models/pharmacy.model';
@Injectable({ providedIn: 'root' })
export class PharmacyService extends ApiService<Medicine> {
  protected readonly resource = 'pharmacy/medicines';
  createMedicine(request: CreateMedicineRequest): Observable<Medicine> {
    return this.create(request);
  }
  stock(id: string, request: StockRequest): Observable<StockTransaction> {
    return this.http.post<StockTransaction>(`${this.url}/${id}/stock`, request);
  }
  balance(id: string): Observable<StockBalance> {
    return this.http.get<StockBalance>(`${this.url}/${id}/stock`);
  }
}
