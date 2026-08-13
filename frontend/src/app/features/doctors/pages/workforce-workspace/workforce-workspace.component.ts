import { CurrencyPipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { finalize, forkJoin, Observable } from 'rxjs';
import { AuthService } from '../../../../core/auth/services/auth.service';
import { NotificationService } from '../../../../core/http/notification.service';
import { ConfirmationDialogComponent } from '../../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { Department } from '../../../departments/models/department.model';
import { DepartmentService } from '../../../departments/services/department.service';
import { Staff } from '../../../staff/models/staff.model';
import { StaffService } from '../../../staff/services/staff.service';
import { Doctor } from '../../models/doctor.model';
import { DoctorService } from '../../services/doctor.service';

import { CountrySelectorComponent } from '../../../../shared/components/country-selector/country-selector.component';

type Tab = 'doctors' | 'departments' | 'staff';

@Component({
  selector: 'app-workforce-workspace',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    CurrencyPipe,
    ConfirmationDialogComponent,
    CountrySelectorComponent,
  ],
  templateUrl: './workforce-workspace.component.html',
  styleUrl: './workforce-workspace.component.scss',
})
export class WorkforceWorkspaceComponent {
  private readonly doctorsApi = inject(DoctorService);
  private readonly departmentsApi = inject(DepartmentService);
  private readonly staffApi = inject(StaffService);
  private readonly notices = inject(NotificationService);
  readonly auth = inject(AuthService);
  readonly tab = signal<Tab>(this.currentTab());
  readonly doctors = signal<Doctor[]>([]);
  readonly departments = signal<Department[]>([]);
  readonly staff = signal<Staff[]>([]);
  readonly search = signal('');
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly createOpen = signal(false);
  readonly pendingDelete = signal<{
    type: 'doctor' | 'department';
    id: string;
    name: string;
  } | null>(null);
  readonly filteredDoctors = computed(() =>
    this.filter(this.doctors(), [
      'employeeNumber',
      'firstName',
      'lastName',
      'specialization',
      'email',
    ]),
  );
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

  readonly specializations: string[] = [
    'Cardiology & Cardiovascular Medicine',
    'Neurology & Neurosurgery',
    'Orthopedic Surgery & Traumatology',
    'Pediatrics & Neonatology',
    'Ophthalmology & Vision Care',
    'Pulmonology & Respiratory Care',
    'Hematology & Oncology',
    'General Internal Medicine',
    'General & Vascular Surgery',
    'Clinical Pathology & Diagnostics',
    'Diagnostic Radiology & Imaging',
    'Dermatology & Cosmetology',
    'Psychiatry & Behavioral Health',
    'Otolaryngology (ENT)',
    'Obstetrics & Gynecology',
    'Anesthesiology & Pain Management',
    'Urology & Kidney Care',
    'Gastroenterology & Hepatology',
  ];

  readonly jobTitles: string[] = [
    'Registered Nurse (RN)',
    'Senior Staff Nurse',
    'Clinical Nurse Specialist',
    'Medical Assistant',
    'Head Nurse / Supervisor',
    'Lab Technician',
    'Radiologic Technologist',
    'Pharmacy Specialist',
    'Medical Receptionist',
    'Billing & Claims Specialist',
    'Hospital Administrator',
    'Facility Manager',
  ];

  readonly filteredDepartments = computed(() =>
    this.filter(this.allAvailableDepartments(), ['code', 'name', 'description']),
  );
  readonly filteredStaff = computed(() =>
    this.filter(this.staff(), ['employeeNumber', 'firstName', 'lastName', 'jobTitle']),
  );

  readonly doctorForm = new FormGroup({
    employeeNumber: new FormControl('', { nonNullable: true, validators: Validators.required }),
    firstName: new FormControl('', { nonNullable: true, validators: Validators.required }),
    lastName: new FormControl('', { nonNullable: true, validators: Validators.required }),
    specialization: new FormControl('', { nonNullable: true, validators: Validators.required }),
    departmentId: new FormControl('', { nonNullable: true, validators: Validators.required }),
    licenseNumber: new FormControl('', { nonNullable: true, validators: Validators.required }),
    phone: new FormControl<string | null>(null),
    email: new FormControl<string | null>(null, Validators.email),
    consultationFee: new FormControl(0, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(0)],
    }),
  });
  readonly departmentForm = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: Validators.required }),
    code: new FormControl('', { nonNullable: true, validators: Validators.required }),
    description: new FormControl<string | null>(null),
  });
  readonly staffForm = new FormGroup({
    employeeNumber: new FormControl('', { nonNullable: true, validators: Validators.required }),
    firstName: new FormControl('', { nonNullable: true, validators: Validators.required }),
    lastName: new FormControl('', { nonNullable: true, validators: Validators.required }),
    jobTitle: new FormControl('', { nonNullable: true, validators: Validators.required }),
    departmentId: new FormControl<string | null>(null),
  });

  constructor() {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    forkJoin({
      doctors: this.doctorsApi.list({ pageSize: 100 }),
      departments: this.departmentsApi.list({ pageSize: 100 }),
      staff: this.staffApi.list({ pageSize: 100 }),
    })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe(({ doctors, departments, staff }) => {
        this.doctors.set(doctors.items);
        this.departments.set(departments.items);
        this.staff.set(staff.items);
      });
  }

  create(): void {
    const tab = this.tab();
    const form =
      tab === 'doctors'
        ? this.doctorForm
        : tab === 'departments'
          ? this.departmentForm
          : this.staffForm;
    if (form.invalid) return form.markAllAsTouched();
    this.saving.set(true);
    const request: Observable<unknown> =
      tab === 'doctors'
        ? this.doctorsApi.createDoctor(this.doctorForm.getRawValue())
        : tab === 'departments'
          ? this.departmentsApi.createDepartment(this.departmentForm.getRawValue())
          : this.staffApi.create(this.staffForm.getRawValue());
    request.pipe(finalize(() => this.saving.set(false))).subscribe(() => {
      this.notices.show(`${this.singular()} created.`, 'success');
      this.createOpen.set(false);
      form.reset();
      this.load();
    });
  }

  setDoctorStatus(doctor: Doctor): void {
    const request =
      doctor.status === 'Active'
        ? this.doctorsApi.deactivate(doctor.id)
        : this.doctorsApi.activate(doctor.id);
    request.subscribe(() => {
      this.notices.show('Doctor status updated.', 'success');
      this.load();
    });
  }

  confirmDelete(): void {
    const target = this.pendingDelete();
    if (!target) return;
    const request =
      target.type === 'doctor'
        ? this.doctorsApi.delete(target.id)
        : this.departmentsApi.delete(target.id);
    this.saving.set(true);
    request.pipe(finalize(() => this.saving.set(false))).subscribe(() => {
      this.notices.show(`${target.name} removed.`, 'success');
      this.pendingDelete.set(null);
      this.load();
    });
  }

  departmentName(id: string | null): string {
    return this.departments().find((x) => x.id === id)?.name ?? 'Unassigned';
  }
  singular(): string {
    return this.tab() === 'doctors'
      ? 'Doctor'
      : this.tab() === 'departments'
        ? 'Department'
        : 'Staff member';
  }
  private currentTab(): Tab {
    return location.pathname.endsWith('/departments')
      ? 'departments'
      : location.pathname.endsWith('/staff')
        ? 'staff'
        : 'doctors';
  }
  private filter<T>(items: T[], keys: Array<keyof T>): T[] {
    const q = this.search().trim().toLowerCase();
    return q
      ? items.filter((item) =>
          keys.some((key) =>
            String(item[key] ?? '')
              .toLowerCase()
              .includes(q),
          ),
        )
      : items;
  }
}
