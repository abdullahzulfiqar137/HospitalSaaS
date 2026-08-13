import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { PagedResponse } from '../../../shared/models/api.models';
import { HospitalDocument } from '../models/document.model';
@Injectable({ providedIn: 'root' })
export class DocumentService {
  private http = inject(HttpClient);
  private url = `${environment.apiUrl}/documents`;
  list(patientId = '', pageNumber = 1, pageSize = 20) {
    let params = new HttpParams().set('pageNumber', pageNumber).set('pageSize', pageSize);
    if (patientId) params = params.set('patientId', patientId);
    return this.http.get<PagedResponse<HospitalDocument>>(this.url, { params });
  }
  upload(patientId: string, file: File) {
    const body = new FormData();
    body.append('patientId', patientId);
    body.append('file', file, file.name);
    return this.http.post<HospitalDocument>(this.url, body);
  }
  delete(id: string) {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
