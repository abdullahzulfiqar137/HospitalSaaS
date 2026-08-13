import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, of } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { DashboardSummary } from '../models/dashboard.model';

const MOCK_DASHBOARD: DashboardSummary = {
  patients: 1240,
  doctors: 48,
  appointmentsToday: 32,
  activeAdmissions: 18,
  unpaidInvoices: 12,
  pendingLabOrders: 9,
  lowStockMedicines: 5,
  pendingNotifications: 4,
};

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private readonly http = inject(HttpClient);

  get(): Observable<DashboardSummary> {
    return this.http.get<DashboardSummary>(`${environment.apiUrl}/dashboard`).pipe(
      catchError(() => of(MOCK_DASHBOARD)),
    );
  }
}
