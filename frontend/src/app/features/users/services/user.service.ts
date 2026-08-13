import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { PageQuery, PagedResponse } from '../../../shared/models/api.models';
import { CreateUserRequest, User } from '../models/user.model';
@Injectable({ providedIn: 'root' })
export class UserService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/users`;

  list(query: PageQuery = {}): Observable<PagedResponse<User>> {
    let params = new HttpParams();
    Object.entries(query).forEach(([key, value]) => {
      if (value !== undefined && value !== '') params = params.set(key, String(value));
    });
    return this.http.get<PagedResponse<User>>(this.url, { params });
  }

  create(request: CreateUserRequest): Observable<User> {
    return this.http.post<User>(this.url, request);
  }
}
