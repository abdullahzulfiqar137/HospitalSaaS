import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize, forkJoin, Observable } from 'rxjs';
import { AuthService } from '../../../../core/auth/services/auth.service';
import { NotificationService } from '../../../../core/http/notification.service';
import { Role } from '../../../roles/models/role.model';
import { RoleService } from '../../../roles/services/role.service';
import { User } from '../../../users/models/user.model';
import { UserService } from '../../../users/services/user.service';

export interface PermissionCategory {
  name: string;
  icon: string;
  permissions: { key: string; label: string }[];
}

export const HOSPITAL_PERMISSION_CATEGORIES: PermissionCategory[] = [
  {
    name: 'Patient Management',
    icon: '🏥',
    permissions: [
      { key: 'Patients.View', label: 'View Patients' },
      { key: 'Patients.Create', label: 'Register Patient' },
      { key: 'Patients.Update', label: 'Update Patient' },
      { key: 'Patients.Delete', label: 'Delete Patient' },
    ],
  },
  {
    name: 'Doctors & Staff',
    icon: '🩺',
    permissions: [
      { key: 'Doctors.View', label: 'View Doctors Directory' },
      { key: 'Doctors.Create', label: 'Add Doctor Profile' },
      { key: 'Doctors.Update', label: 'Update Doctor' },
      { key: 'Doctors.Delete', label: 'Remove Doctor' },
      { key: 'Staff.View', label: 'View Workforce Staff' },
      { key: 'Staff.Create', label: 'Add Staff Member' },
    ],
  },
  {
    name: 'Appointments & Scheduling',
    icon: '📅',
    permissions: [
      { key: 'Appointments.View', label: 'View Appointments' },
      { key: 'Appointments.Create', label: 'Book Appointment' },
      { key: 'Appointments.Update', label: 'Reschedule / Confirm' },
      { key: 'Appointments.Cancel', label: 'Cancel Appointment' },
    ],
  },
  {
    name: 'Inpatient Admissions & Wards',
    icon: '🛌',
    permissions: [
      { key: 'Admissions.View', label: 'View Bed Admissions' },
      { key: 'Admissions.Create', label: 'Admit Patient' },
      { key: 'Admissions.Discharge', label: 'Discharge Patient' },
    ],
  },
  {
    name: 'Clinical & Prescriptions',
    icon: '📋',
    permissions: [
      { key: 'MedicalRecords.View', label: 'View Medical Records' },
      { key: 'MedicalRecords.Create', label: 'Create Record' },
      { key: 'Prescriptions.View', label: 'View Prescriptions' },
      { key: 'Prescriptions.Create', label: 'Issue Prescription' },
    ],
  },
  {
    name: 'Diagnostics & Laboratory',
    icon: '🧪',
    permissions: [
      { key: 'Laboratory.View', label: 'View Lab Workspace' },
      { key: 'Laboratory.CreateOrder', label: 'Order Lab Tests' },
      { key: 'Radiology.View', label: 'View Scans' },
      { key: 'Radiology.CreateOrder', label: 'Order Scans' },
    ],
  },
  {
    name: 'Pharmacy & Inventory',
    icon: '💊',
    permissions: [
      { key: 'Pharmacy.View', label: 'View Pharmacy' },
      { key: 'Pharmacy.Create', label: 'Add Medicine Stock' },
      { key: 'Inventory.View', label: 'View Medical Supplies' },
      { key: 'Inventory.Create', label: 'Manage Stock' },
      { key: 'Suppliers.View', label: 'View Vendors' },
      { key: 'PurchaseOrders.Create', label: 'Create Purchase Orders' },
    ],
  },
  {
    name: 'Billing & Payments',
    icon: '💳',
    permissions: [
      { key: 'Billing.View', label: 'View Patient Invoices' },
      { key: 'Billing.Create', label: 'Generate Invoice' },
      { key: 'Payments.View', label: 'View Payments' },
      { key: 'Payments.Create', label: 'Record Payment' },
    ],
  },
  {
    name: 'Administration & System',
    icon: '🔐',
    permissions: [
      { key: 'Roles.View', label: 'View Roles & Policies' },
      { key: 'Roles.Create', label: 'Create Role' },
      { key: 'Roles.Update', label: 'Edit Role' },
      { key: 'Roles.Delete', label: 'Delete Role' },
      { key: 'Tenants.View', label: 'View Tenant Info' },
      { key: 'Dashboard.View', label: 'View Dashboard' },
    ],
  },
];

@Component({
  selector: 'app-access-workspace',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './access-workspace.component.html',
  styleUrl: './access-workspace.component.scss',
})
export class AccessWorkspaceComponent {
  private readonly router = inject(Router);
  private readonly usersApi = inject(UserService);
  private readonly rolesApi = inject(RoleService);
  private readonly notices = inject(NotificationService);
  readonly auth = inject(AuthService);

  readonly mode = signal<'users' | 'roles'>(this.router.url.includes('roles') ? 'roles' : 'users');
  readonly users = signal<User[]>([]);
  readonly roles = signal<Role[]>([]);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly modal = signal(false);

  readonly categories = HOSPITAL_PERMISSION_CATEGORIES;
  readonly selectedPermissions = signal<string[]>([]);

  readonly userForm = new FormGroup({
    email: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.email],
    }),
    password: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(8)],
    }),
    roleIds: new FormControl<string[]>([], { nonNullable: true, validators: Validators.required }),
  });

  readonly roleForm = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: Validators.required }),
  });

  constructor() {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    forkJoin({
      users: this.usersApi.list({ pageSize: 100 }),
      roles: this.rolesApi.list({ pageSize: 100 }),
    })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe((x) => {
        this.users.set(x.users.items);
        this.roles.set(x.roles.items);
      });
  }

  openModal(): void {
    this.selectedPermissions.set([]);
    this.userForm.reset({ roleIds: [] });
    this.roleForm.reset();
    this.modal.set(true);
  }

  togglePermission(key: string, checked: boolean): void {
    const current = this.selectedPermissions();
    if (checked) {
      if (!current.includes(key)) {
        this.selectedPermissions.set([...current, key]);
      }
    } else {
      this.selectedPermissions.set(current.filter((k) => k !== key));
    }
  }

  toggleCategory(category: PermissionCategory): void {
    const keys = category.permissions.map((p) => p.key);
    const current = this.selectedPermissions();
    const allSelected = keys.every((k) => current.includes(k));
    if (allSelected) {
      this.selectedPermissions.set(current.filter((k) => !keys.includes(k)));
    } else {
      const merged = new Set([...current, ...keys]);
      this.selectedPermissions.set(Array.from(merged));
    }
  }

  applyPresetRole(roleType: string): void {
    let perms: string[] = [];
    if (roleType === 'admin') {
      perms = this.categories.flatMap((c) => c.permissions.map((p) => p.key));
      this.roleForm.controls.name.setValue('Hospital Admin');
    } else if (roleType === 'doctor') {
      perms = [
        'Patients.View', 'Patients.Create', 'Patients.Update',
        'Doctors.View', 'Appointments.View', 'Appointments.Create', 'Appointments.Update',
        'Admissions.View', 'Admissions.Create',
        'MedicalRecords.View', 'MedicalRecords.Create',
        'Prescriptions.View', 'Prescriptions.Create',
        'Laboratory.View', 'Laboratory.CreateOrder',
        'Radiology.View', 'Radiology.CreateOrder',
        'Dashboard.View'
      ];
      this.roleForm.controls.name.setValue('Doctor / Consultant');
    } else if (roleType === 'nurse') {
      perms = [
        'Patients.View', 'Patients.Update',
        'Appointments.View', 'Admissions.View', 'Admissions.Create',
        'MedicalRecords.View', 'Prescriptions.View',
        'Dashboard.View'
      ];
      this.roleForm.controls.name.setValue('Nurse');
    } else if (roleType === 'lab') {
      perms = [
        'Patients.View', 'Laboratory.View', 'Laboratory.CreateOrder',
        'Radiology.View', 'Radiology.CreateOrder', 'MedicalRecords.View',
        'Dashboard.View'
      ];
      this.roleForm.controls.name.setValue('Lab Technician');
    } else if (roleType === 'pharmacist') {
      perms = [
        'Patients.View', 'Pharmacy.View', 'Pharmacy.Create',
        'Inventory.View', 'Inventory.Create', 'Prescriptions.View',
        'Dashboard.View'
      ];
      this.roleForm.controls.name.setValue('Pharmacist');
    } else if (roleType === 'billing') {
      perms = [
        'Patients.View', 'Billing.View', 'Billing.Create',
        'Payments.View', 'Payments.Create', 'Dashboard.View'
      ];
      this.roleForm.controls.name.setValue('Billing Clerk');
    } else if (roleType === 'receptionist') {
      perms = [
        'Patients.View', 'Patients.Create', 'Patients.Update',
        'Appointments.View', 'Appointments.Create', 'Appointments.Update',
        'Dashboard.View'
      ];
      this.roleForm.controls.name.setValue('Receptionist');
    }
    this.selectedPermissions.set(perms);
  }

  toggleRole(id: string, checked: boolean): void {
    const ids = this.userForm.controls.roleIds.value;
    this.userForm.controls.roleIds.setValue(checked ? [...ids, id] : ids.filter((x) => x !== id));
  }

  save(): void {
    if (this.mode() === 'users') {
      if (this.userForm.invalid) return this.userForm.markAllAsTouched();
      this.saving.set(true);
      this.usersApi.create(this.userForm.getRawValue())
        .pipe(finalize(() => this.saving.set(false)))
        .subscribe(() => {
          this.notices.show('User created successfully.', 'success');
          this.modal.set(false);
          this.userForm.reset({ roleIds: [] });
          this.load();
        });
    } else {
      if (this.roleForm.invalid) return this.roleForm.markAllAsTouched();
      if (this.selectedPermissions().length === 0) {
        return this.notices.show('Please select at least one permission.', 'error');
      }
      this.saving.set(true);
      this.rolesApi.create({
        name: this.roleForm.controls.name.value,
        permissions: this.selectedPermissions(),
      })
        .pipe(finalize(() => this.saving.set(false)))
        .subscribe(() => {
          this.notices.show('Role created successfully.', 'success');
          this.modal.set(false);
          this.roleForm.reset();
          this.selectedPermissions.set([]);
          this.load();
        });
    }
  }
}
