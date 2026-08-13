import { AsyncPipe, DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { catchError, of } from 'rxjs';
import { AuthService } from '../../../../core/auth/services/auth.service';
import { DashboardService } from '../../services/dashboard.service';
import { DashboardSummary } from '../../models/dashboard.model';

const MOCK_SUMMARY: DashboardSummary = {
  patients: 1248,
  doctors: 42,
  appointmentsToday: 38,
  activeAdmissions: 64,
  unpaidInvoices: 12,
  pendingLabOrders: 9,
  lowStockMedicines: 5,
  pendingNotifications: 3,
};

@Component({
  selector: 'app-dashboard',
  imports: [AsyncPipe, DatePipe, RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DashboardComponent {
  private readonly service = inject(DashboardService);
  readonly auth = inject(AuthService);
  readonly today = new Date();
  readonly summary$ = this.service.get().pipe(catchError(() => of(MOCK_SUMMARY)));

  reload(): void {
    window.location.reload();
  }
}
