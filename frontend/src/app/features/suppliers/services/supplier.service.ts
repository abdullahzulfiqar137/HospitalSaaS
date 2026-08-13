import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/http/api.service';
import { CreateSupplierRequest, Supplier } from '../models/supplier.model';
@Injectable({ providedIn: 'root' })
export class SupplierService extends ApiService<Supplier> {
  protected readonly resource = 'suppliers';

  createSupplier(request: CreateSupplierRequest): Observable<Supplier> {
    return this.create(request);
  }
}
