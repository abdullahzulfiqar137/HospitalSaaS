import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/http/api.service';
import {
  CreateDepartmentRequest,
  Department,
  UpdateDepartmentRequest,
} from '../models/department.model';

@Injectable({ providedIn: 'root' })
export class DepartmentService extends ApiService<Department> {
  protected readonly resource = 'departments';
  createDepartment(request: CreateDepartmentRequest): Observable<Department> {
    return this.create(request);
  }
  update(id: string, request: UpdateDepartmentRequest): Observable<Department> {
    return this.http.put<Department>(`${this.url}/${id}`, request);
  }
}
