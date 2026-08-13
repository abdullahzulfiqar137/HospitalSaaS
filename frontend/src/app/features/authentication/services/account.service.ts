import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../../environments/environment';
@Injectable({ providedIn: 'root' })
export class AccountService {
  private http = inject(HttpClient);
  changePassword(currentPassword: string, newPassword: string) {
    return this.http.post<void>(`${environment.apiUrl}/auth/change-password`, {
      currentPassword,
      newPassword,
    });
  }
}
