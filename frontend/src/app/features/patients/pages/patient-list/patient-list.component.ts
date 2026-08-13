import { AsyncPipe, DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import {
  BehaviorSubject,
  catchError,
  combineLatest,
  debounceTime,
  map,
  of,
  startWith,
  switchMap,
} from 'rxjs';
import { AuthService } from '../../../../core/auth/services/auth.service';
import { NotificationService } from '../../../../core/http/notification.service';
import { ConfirmationDialogComponent } from '../../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { PatientService } from '../../services/patient.service';
@Component({
  selector: 'app-patient-list',
  imports: [AsyncPipe, DatePipe, ReactiveFormsModule, RouterLink, ConfirmationDialogComponent],
  templateUrl: './patient-list.component.html',
  styleUrl: './patient-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PatientListComponent {
  private readonly service = inject(PatientService);
  readonly auth = inject(AuthService);
  private readonly notices = inject(NotificationService);
  readonly search = new FormControl('', { nonNullable: true });
  readonly page = signal(1);
  readonly pendingDelete = signal<string | null>(null);
  readonly deleting = signal(false);
  private readonly refresh = new BehaviorSubject(0);
  readonly data$ = combineLatest([
    this.search.valueChanges.pipe(startWith(''), debounceTime(250)),
    this.refresh,
  ]).pipe(
    switchMap(([query]) =>
      this.service.list({ query, pageNumber: this.page(), pageSize: 10 }).pipe(
        map((data) => ({ data, error: false })),
        catchError(() => of({ data: null, error: true })),
      ),
    ),
  );
  changePage(value: number) {
    this.page.set(value);
    this.refresh.next(0);
  }
  remove(id: string) {
    this.pendingDelete.set(id);
  }
  confirmDelete() {
    const id = this.pendingDelete();
    if (!id) return;
    this.deleting.set(true);
    this.service.delete(id).subscribe({
      next: () => {
        this.notices.show('Patient deleted successfully.', 'success');
        this.pendingDelete.set(null);
        this.deleting.set(false);
        this.refresh.next(0);
      },
      error: () => this.deleting.set(false),
    });
  }
}
