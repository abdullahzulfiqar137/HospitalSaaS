import { DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, forkJoin } from 'rxjs';
import { AuthService } from '../../../../core/auth/services/auth.service';
import { NotificationService } from '../../../../core/http/notification.service';
import { Patient } from '../../../patients/models/patient.model';
import { PatientService } from '../../../patients/services/patient.service';
import { InsurancePolicy } from '../../models/insurance.model';
import { InsuranceService } from '../../services/insurance.service';

@Component({
  selector: 'app-insurance-workspace',
  imports: [ReactiveFormsModule, DatePipe],
  templateUrl: './insurance-workspace.component.html',
  styleUrl: './insurance-workspace.component.scss',
})
export class InsuranceWorkspaceComponent {
  private readonly api = inject(InsuranceService);
  private readonly patientsApi = inject(PatientService);
  private readonly notices = inject(NotificationService);
  readonly auth = inject(AuthService);
  readonly policies = signal<InsurancePolicy[]>([]);
  readonly patients = signal<Patient[]>([]);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly modal = signal(false);
  readonly patientFilter = signal('');
  readonly form = new FormGroup({
    patientId: new FormControl('', { nonNullable: true, validators: Validators.required }),
    provider: new FormControl('', { nonNullable: true, validators: Validators.required }),
    policyNumber: new FormControl('', { nonNullable: true, validators: Validators.required }),
    validFrom: new FormControl('', { nonNullable: true, validators: Validators.required }),
    validTo: new FormControl('', { nonNullable: true, validators: Validators.required }),
  });
  constructor() {
    this.load();
  }
  load(): void {
    this.loading.set(true);
    forkJoin({
      policies: this.api.list({ patientId: this.patientFilter() || undefined, pageSize: 100 }),
      patients: this.patientsApi.list({ pageSize: 100 }),
    })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe((data) => {
        this.policies.set(data.policies.items);
        this.patients.set(data.patients.items);
      });
  }
  save(): void {
    if (this.form.invalid) return this.form.markAllAsTouched();
    const value = this.form.getRawValue();
    if (value.validTo < value.validFrom) {
      this.notices.show('Valid-to date must be after valid-from date.', 'error');
      return;
    }
    this.saving.set(true);
    this.api
      .createPolicy(value)
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe(() => {
        this.notices.show('Insurance policy added.', 'success');
        this.modal.set(false);
        this.form.reset();
        this.load();
      });
  }
  patientName(id: string): string {
    const p = this.patients().find((x) => x.id === id);
    return p ? `${p.firstName} ${p.lastName}` : id;
  }
  status(policy: InsurancePolicy): 'Active' | 'Expired' | 'Upcoming' {
    const now = new Date().toISOString().slice(0, 10);
    return policy.validFrom > now ? 'Upcoming' : policy.validTo < now ? 'Expired' : 'Active';
  }
}
