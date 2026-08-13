import { DatePipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, forkJoin } from 'rxjs';
import { AuthService } from '../../../../core/auth/services/auth.service';
import { NotificationService } from '../../../../core/http/notification.service';
import { Department } from '../../../departments/models/department.model';
import { DepartmentService } from '../../../departments/services/department.service';
import { Doctor } from '../../../doctors/models/doctor.model';
import { DoctorService } from '../../../doctors/services/doctor.service';
import { Patient } from '../../../patients/models/patient.model';
import { PatientService } from '../../../patients/services/patient.service';
import { Appointment, AppointmentAction, AppointmentStatus } from '../../models/appointment.model';
import { AppointmentService } from '../../services/appointment.service';

@Component({
  selector: 'app-appointment-workspace',
  imports: [ReactiveFormsModule, DatePipe],
  templateUrl: './appointment-workspace.component.html',
  styleUrl: './appointment-workspace.component.scss',
})
export class AppointmentWorkspaceComponent {
  private readonly api = inject(AppointmentService);
  private readonly patientsApi = inject(PatientService);
  private readonly doctorsApi = inject(DoctorService);
  private readonly departmentsApi = inject(DepartmentService);
  private readonly notices = inject(NotificationService);
  readonly auth = inject(AuthService);
  readonly appointments = signal<Appointment[]>([]);
  readonly patients = signal<Patient[]>([]);
  readonly doctors = signal<Doctor[]>([]);
  readonly departments = signal<Department[]>([]);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly createOpen = signal(false);
  readonly actionOpen = signal(false);
  readonly selected = signal<Appointment | null>(null);
  readonly actionName = signal<AppointmentAction>('Confirm');
  readonly dateFilter = signal('');
  readonly statusFilter = signal<AppointmentStatus | ''>('');
  readonly defaultDepartments: Department[] = [
    { id: '00000000-0000-0000-0000-000000000101', name: 'Cardiology & Cardiovascular Care', code: 'CARD', description: 'Heart and vascular care', status: 'Active' },
    { id: '00000000-0000-0000-0000-000000000102', name: 'Neurology & Neurosurgery', code: 'NEURO', description: 'Brain and neurology', status: 'Active' },
    { id: '00000000-0000-0000-0000-000000000103', name: 'Orthopedics & Traumatology', code: 'ORTHO', description: 'Bones and joint care', status: 'Active' },
    { id: '00000000-0000-0000-0000-000000000104', name: 'Pediatrics & Child Health', code: 'PEDS', description: 'Child care', status: 'Active' },
    { id: '00000000-0000-0000-0000-000000000105', name: 'Emergency & Trauma Care', code: 'EMERG', description: '24/7 Emergency care', status: 'Active' },
    { id: '00000000-0000-0000-0000-000000000106', name: 'General Internal Medicine', code: 'MED', description: 'Internal medicine', status: 'Active' },
    { id: '00000000-0000-0000-0000-000000000107', name: 'Pathology & Clinical Lab', code: 'PATH', description: 'Diagnostic pathology', status: 'Active' },
    { id: '00000000-0000-0000-0000-000000000108', name: 'Radiology & Medical Imaging', code: 'RAD', description: 'Medical imaging', status: 'Active' },
    { id: '00000000-0000-0000-0000-000000000109', name: 'General Surgery', code: 'SURG', description: 'Surgical OR', status: 'Active' },
    { id: '00000000-0000-0000-0000-000000000110', name: 'Obstetrics & Gynecology', code: 'OBGYN', description: 'Women health & maternity', status: 'Active' },
  ];

  readonly allAvailableDepartments = computed(() => {
    const list = this.departments();
    return list.length > 0 ? list : this.defaultDepartments;
  });

  readonly visible = computed(() => this.appointments());

  readonly form = new FormGroup({
    patientId: new FormControl('', { nonNullable: true, validators: Validators.required }),
    doctorId: new FormControl('', { nonNullable: true, validators: Validators.required }),
    departmentId: new FormControl('', { nonNullable: true, validators: Validators.required }),
    appointmentDateTimeUtc: new FormControl('', {
      nonNullable: true,
      validators: Validators.required,
    }),
    durationMinutes: new FormControl(30, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(5)],
    }),
    appointmentType: new FormControl('Consultation', {
      nonNullable: true,
      validators: Validators.required,
    }),
    reason: new FormControl('', { nonNullable: true, validators: Validators.required }),
    notes: new FormControl<string | null>(null),
  });
  readonly actionForm = new FormGroup({
    reason: new FormControl<string | null>(null),
    appointmentDateTimeUtc: new FormControl<string | null>(null),
    durationMinutes: new FormControl<number | null>(null),
  });

  constructor() {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    forkJoin({
      appointments: this.api.search({
        date: this.dateFilter() || undefined,
        status: this.statusFilter() || undefined,
        pageSize: 100,
      }),
      patients: this.patientsApi.list({ pageSize: 100 }),
      doctors: this.doctorsApi.list({ pageSize: 100 }),
      departments: this.departmentsApi.list({ pageSize: 100 }),
    })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe(({ appointments, patients, doctors, departments }) => {
        this.appointments.set(appointments.items);
        this.patients.set(patients.items);
        this.doctors.set(doctors.items);
        this.departments.set(departments.items);
      });
  }

  book(): void {
    if (this.form.invalid) return this.form.markAllAsTouched();
    const value = this.form.getRawValue();
    this.saving.set(true);
    this.api
      .book({
        ...value,
        appointmentDateTimeUtc: new Date(value.appointmentDateTimeUtc).toISOString(),
      })
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe(() => {
        this.notices.show('Appointment booked.', 'success');
        this.createOpen.set(false);
        this.form.reset({
          patientId: '',
          doctorId: '',
          departmentId: '',
          appointmentDateTimeUtc: '',
          durationMinutes: 30,
          appointmentType: 'Consultation',
          reason: '',
          notes: null,
        });
        this.load();
      });
  }

  openAction(appointment: Appointment, action: AppointmentAction): void {
    this.selected.set(appointment);
    this.actionName.set(action);
    this.actionForm.reset({
      reason: null,
      appointmentDateTimeUtc:
        action === 'Reschedule' ? this.localDate(appointment.appointmentDateTimeUtc) : null,
      durationMinutes: action === 'Reschedule' ? appointment.durationMinutes : null,
    });
    this.actionOpen.set(true);
  }

  runAction(): void {
    const appointment = this.selected();
    if (!appointment) return;
    const value = this.actionForm.getRawValue();
    if (this.actionName() === 'Cancel' && !value.reason?.trim())
      return this.notices.show('Cancellation reason is required.', 'error');
    if (
      this.actionName() === 'Reschedule' &&
      (!value.appointmentDateTimeUtc || !value.durationMinutes)
    )
      return this.notices.show('New date and duration are required.', 'error');
    this.saving.set(true);
    this.api
      .action(appointment.id, {
        action: this.actionName(),
        reason: value.reason,
        appointmentDateTimeUtc: value.appointmentDateTimeUtc
          ? new Date(value.appointmentDateTimeUtc).toISOString()
          : null,
        durationMinutes: value.durationMinutes,
      })
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe(() => {
        this.notices.show(`${this.actionName()} completed.`, 'success');
        this.actionOpen.set(false);
        this.selected.set(null);
        this.load();
      });
  }

  patientName(id: string): string {
    const x = this.patients().find((p) => p.id === id);
    return x ? `${x.firstName} ${x.lastName}` : id;
  }
  doctorName(id: string): string {
    const x = this.doctors().find((d) => d.id === id);
    return x ? `Dr. ${x.firstName} ${x.lastName}` : id;
  }
  departmentName(id: string): string {
    return this.departments().find((d) => d.id === id)?.name ?? id;
  }
  actions(status: AppointmentStatus): AppointmentAction[] {
    return status === 'Scheduled'
      ? ['Confirm', 'Reschedule', 'Cancel']
      : status === 'Confirmed'
        ? ['CheckIn', 'Reschedule', 'Cancel', 'NoShow']
        : status === 'CheckedIn'
          ? ['Start', 'Cancel']
          : status === 'InProgress'
            ? ['Complete']
            : [];
  }
  private localDate(value: string): string {
    const date = new Date(value);
    return new Date(date.getTime() - date.getTimezoneOffset() * 60000).toISOString().slice(0, 16);
  }

  formatPatientLabel(patient: { firstName: string; lastName: string; patientNumber: string }): string {
    const cleanLast = (patient.lastName || '').replace(/\d+/g, '').trim();
    const name = `${patient.firstName} ${cleanLast}`.trim();
    return `${name} (${patient.patientNumber})`;
  }

  formatDoctorLabel(doctor: { firstName: string; lastName: string; specialization?: string }): string {
    const cleanLast = (doctor.lastName || '').replace(/\d+/g, '').trim();
    const name = `Dr. ${doctor.firstName} ${cleanLast}`.trim();
    return doctor.specialization ? `${name} — ${doctor.specialization}` : name;
  }
}
