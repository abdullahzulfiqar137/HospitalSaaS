import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NavigationEnd, Router } from '@angular/router';
import { filter, finalize, forkJoin, Observable } from 'rxjs';
import { AuthService } from '../../../../core/auth/services/auth.service';
import { NotificationService } from '../../../../core/http/notification.service';
import { Doctor } from '../../../doctors/models/doctor.model';
import { DoctorService } from '../../../doctors/services/doctor.service';
import { LabOrder, LabTest } from '../../../laboratory/models/laboratory.model';
import { LaboratoryService } from '../../../laboratory/services/laboratory.service';
import { Patient } from '../../../patients/models/patient.model';
import { PatientService } from '../../../patients/services/patient.service';
import { RadiologyOrder, RadiologyTest } from '../../../radiology/models/radiology.model';
import { RadiologyService } from '../../../radiology/services/radiology.service';

type Mode = 'laboratory' | 'radiology';
type Panel = 'orders' | 'catalog';

@Component({
  selector: 'app-diagnostics-workspace',
  imports: [ReactiveFormsModule, DatePipe, DecimalPipe],
  templateUrl: './diagnostics-workspace.component.html',
  styleUrl: './diagnostics-workspace.component.scss',
})
export class DiagnosticsWorkspaceComponent {
  private readonly router = inject(Router);
  private readonly labsApi = inject(LaboratoryService);
  private readonly radiologyApi = inject(RadiologyService);
  private readonly patientsApi = inject(PatientService);
  private readonly doctorsApi = inject(DoctorService);
  private readonly notices = inject(NotificationService);
  readonly auth = inject(AuthService);
  readonly mode = signal<Mode>('laboratory');
  readonly panel = signal<Panel>('orders');
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly modal = signal<'order' | 'test' | 'result' | 'report' | null>(null);
  readonly patients = signal<Patient[]>([]);
  readonly doctors = signal<Doctor[]>([]);
  readonly labTests = signal<LabTest[]>([]);
  readonly labOrders = signal<LabOrder[]>([]);
  readonly radiologyTests = signal<RadiologyTest[]>([]);
  readonly radiologyOrders = signal<RadiologyOrder[]>([]);
  readonly selectedLabTests = signal<LabTest[]>([]);
  readonly selectedLabOrder = signal<LabOrder | null>(null);
  readonly selectedLabItemId = signal('');
  readonly selectedRadiologyOrder = signal<RadiologyOrder | null>(null);

  readonly orderForm = new FormGroup({
    patientId: new FormControl('', { nonNullable: true, validators: Validators.required }),
    doctorId: new FormControl('', { nonNullable: true, validators: Validators.required }),
    testId: new FormControl('', { nonNullable: true }),
  });
  readonly testForm = new FormGroup({
    code: new FormControl('', { nonNullable: true, validators: Validators.required }),
    name: new FormControl('', { nonNullable: true, validators: Validators.required }),
    category: new FormControl('', { nonNullable: true }),
    modality: new FormControl('', { nonNullable: true }),
    price: new FormControl(0, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(0)],
    }),
    normalRange: new FormControl<string | null>(null),
    unit: new FormControl<string | null>(null),
  });
  readonly resultForm = new FormGroup({
    result: new FormControl('', { nonNullable: true, validators: Validators.required }),
    unit: new FormControl<string | null>(null),
    referenceRange: new FormControl<string | null>(null),
  });
  readonly reportForm = new FormGroup({
    report: new FormControl('', { nonNullable: true, validators: Validators.required }),
  });

  constructor() {
    this.syncRoute();
    this.router.events.pipe(filter((event) => event instanceof NavigationEnd)).subscribe(() => {
      this.syncRoute();
      this.load();
    });
    this.load();
  }

  load(): void {
    this.loading.set(true);
    forkJoin({
      patients: this.patientsApi.list({ pageSize: 100 }),
      doctors: this.doctorsApi.list({ pageSize: 100 }),
      labTests: this.labsApi.tests('', 1, 100),
      labOrders: this.labsApi.orders(undefined, undefined, 1, 100),
      radiologyTests: this.radiologyApi.tests('', 1, 100),
      radiologyOrders: this.radiologyApi.orders(undefined, 1, 100),
    })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe((data) => {
        this.patients.set(data.patients.items);
        this.doctors.set(data.doctors.items);
        this.labTests.set(data.labTests.items);
        this.labOrders.set(data.labOrders.items);
        this.radiologyTests.set(data.radiologyTests.items);
        this.radiologyOrders.set(data.radiologyOrders.items);
      });
  }

  openOrder(): void {
    this.orderForm.reset({ patientId: '', doctorId: '', testId: '' });
    this.selectedLabTests.set([]);
    this.modal.set('order');
  }
  addLabTest(): void {
    const test = this.labTests().find((item) => item.id === this.orderForm.controls.testId.value);
    if (test && !this.selectedLabTests().some((item) => item.id === test.id))
      this.selectedLabTests.update((items) => [...items, test]);
    this.orderForm.controls.testId.setValue('');
  }
  removeLabTest(id: string): void {
    this.selectedLabTests.update((items) => items.filter((item) => item.id !== id));
  }
  saveOrder(): void {
    if (this.orderForm.invalid) return this.orderForm.markAllAsTouched();
    const value = this.orderForm.getRawValue();
    this.saving.set(true);
    const request: Observable<unknown> =
      this.mode() === 'laboratory'
        ? this.labsApi.createOrder({
            patientId: value.patientId,
            doctorId: value.doctorId,
            tests: this.selectedLabTests().map((test) => ({
              code: test.code,
              name: test.name,
              labTestId: test.id,
            })),
          })
        : this.radiologyApi.createOrder({
            patientId: value.patientId,
            doctorId: value.doctorId,
            radiologyTestId: value.testId || null,
            procedure: this.radiologyTests().find((test) => test.id === value.testId)?.name ?? '',
          });
    request.pipe(finalize(() => this.saving.set(false))).subscribe(() => {
      this.notices.show('Diagnostic order created.', 'success');
      this.modal.set(null);
      this.load();
    });
  }
  saveTest(): void {
    if (this.testForm.invalid) return this.testForm.markAllAsTouched();
    const value = this.testForm.getRawValue();
    this.saving.set(true);
    const request: Observable<unknown> =
      this.mode() === 'laboratory'
        ? this.labsApi.createTest({
            name: value.name,
            code: value.code,
            category: value.category,
            price: value.price,
            normalRange: value.normalRange,
            unit: value.unit,
          })
        : this.radiologyApi.createTest({
            name: value.name,
            code: value.code,
            modality: value.modality,
            price: value.price,
          });
    request.pipe(finalize(() => this.saving.set(false))).subscribe(() => {
      this.notices.show('Diagnostic test added.', 'success');
      this.modal.set(null);
      this.testForm.reset({
        code: '',
        name: '',
        category: '',
        modality: '',
        price: 0,
        normalRange: null,
        unit: null,
      });
      this.load();
    });
  }
  formatPatientLabel(patient: { firstName: string; lastName: string; patientNumber: string }): string {
    const cleanLast = (patient.lastName || '').replace(/\d+/g, '').trim();
    const name = `${patient.firstName} ${cleanLast}`.trim();
    return `${name} (${patient.patientNumber})`;
  }

  labAction(order: LabOrder, action: string): void {
    this.saving.set(true);
    this.labsApi
      .action(order.id, action)
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe(() => {
        this.notices.show(`${action} completed.`, 'success');
        this.load();
      });
  }
  openResult(order: LabOrder, itemId: string): void {
    const item = order.items.find((entry) => entry.id === itemId);
    this.selectedLabOrder.set(order);
    this.selectedLabItemId.set(itemId);
    this.resultForm.reset({
      result: item?.result ?? '',
      unit: item?.unit ?? null,
      referenceRange: item?.referenceRange ?? null,
    });
    this.modal.set('result');
  }
  saveResult(): void {
    if (this.resultForm.invalid || !this.selectedLabOrder()) return;
    this.saving.set(true);
    this.labsApi
      .recordResult(this.selectedLabOrder()!.id, {
        itemId: this.selectedLabItemId(),
        ...this.resultForm.getRawValue(),
      })
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe(() => {
        this.notices.show('Lab result recorded.', 'success');
        this.modal.set(null);
        this.load();
      });
  }
  openReport(order: RadiologyOrder): void {
    this.selectedRadiologyOrder.set(order);
    this.reportForm.reset({ report: order.report ?? '' });
    this.modal.set('report');
  }
  saveReport(): void {
    if (this.reportForm.invalid || !this.selectedRadiologyOrder()) return;
    this.saving.set(true);
    this.radiologyApi
      .recordReport(this.selectedRadiologyOrder()!.id, this.reportForm.controls.report.value)
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe(() => {
        this.notices.show('Radiology report recorded.', 'success');
        this.modal.set(null);
        this.load();
      });
  }
  patientName(id: string): string {
    const patient = this.patients().find((item) => item.id === id);
    return patient ? `${patient.firstName} ${patient.lastName}` : id;
  }
  doctorName(id: string): string {
    const doctor = this.doctors().find((item) => item.id === id);
    return doctor ? `Dr. ${doctor.firstName} ${doctor.lastName}` : id;
  }
  private syncRoute(): void {
    this.mode.set(this.router.url.includes('radiology') ? 'radiology' : 'laboratory');
    this.panel.set(this.router.url.includes('tests') ? 'catalog' : 'orders');
  }
}
