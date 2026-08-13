import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { catchError, finalize, forkJoin, of } from 'rxjs';
import { NotificationService } from '../../../../core/http/notification.service';
import { Admission, AdmissionHistory } from '../../models/admission.model';
import { AdmissionService } from '../../services/admission.service';

@Component({
  selector: 'app-admission-workflow',
  imports: [ReactiveFormsModule, DatePipe],
  templateUrl: './admission-workflow.component.html',
  styleUrl: './admission-workflow.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdmissionWorkflowComponent {
  private readonly api = inject(AdmissionService);
  private readonly notice = inject(NotificationService);

  readonly record = signal<Admission | null>(null);
  readonly history = signal<AdmissionHistory[]>([]);
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly lookupError = signal('');
  readonly transferOpen = signal(false);
  readonly dischargeOpen = signal(false);
  readonly lookup = new FormControl('', { nonNullable: true, validators: [Validators.required] });
  readonly form = new FormGroup({
    patientId: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    doctorId: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    admissionType: new FormControl('Emergency', {
      nonNullable: true,
      validators: [Validators.required],
    }),
    reason: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    wardId: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    roomId: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    bedId: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
  });
  readonly transferForm = new FormGroup({
    wardId: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    roomId: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    bedId: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    reason: new FormControl('', { nonNullable: true }),
  });

  find(): void {
    const id = this.lookup.value.trim();
    if (!id) {
      this.lookup.markAsTouched();
      return;
    }
    this.loading.set(true);
    this.lookupError.set('');
    forkJoin([this.api.get(id), this.api.history(id)])
      .pipe(
        catchError(() => {
          this.lookupError.set('Admission was not found or could not be loaded.');
          return of(null);
        }),
        finalize(() => this.loading.set(false)),
      )
      .subscribe((result) => {
        if (result) {
          this.record.set(result[0]);
          this.history.set(result[1]);
        }
      });
  }

  admit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.api
      .admit(this.form.getRawValue())
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe((admission) => {
        this.notice.show('Patient admitted successfully.', 'success');
        this.lookup.setValue(admission.id);
        this.form.reset({ admissionType: 'Emergency' });
        this.find();
      });
  }

  openTransfer(): void {
    const admission = this.record();
    if (!admission) return;
    this.transferForm.reset({
      wardId: admission.wardId,
      roomId: admission.roomId,
      bedId: admission.bedId,
      reason: '',
    });
    this.transferOpen.set(true);
  }

  transfer(): void {
    const id = this.record()?.id;
    if (!id || this.transferForm.invalid) {
      this.transferForm.markAllAsTouched();
      return;
    }
    const value = this.transferForm.getRawValue();
    this.saving.set(true);
    this.api
      .transfer(id, { ...value, reason: value.reason || null })
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe(() => {
        this.notice.show('Admission transferred successfully.', 'success');
        this.transferOpen.set(false);
        this.find();
      });
  }

  discharge(): void {
    const id = this.record()?.id;
    if (!id) return;
    this.saving.set(true);
    this.api
      .discharge(id)
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe(() => {
        this.notice.show('Patient discharged successfully.', 'success');
        this.dischargeOpen.set(false);
        this.find();
      });
  }
}
