import { ChangeDetectionStrategy, Component, inject, input, output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/auth/services/auth.service';

interface NavItem {
  path: string;
  label: string;
  icon: string;
  permission: string;
  group: 'Overview' | 'Clinical care' | 'Diagnostics' | 'Finance & supply' | 'Administration';
}

@Component({
  selector: 'app-sidebar',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SidebarComponent {
  readonly auth = inject(AuthService);
  readonly collapsed = input(false);
  readonly toggle = output<void>();

  readonly links: NavItem[] = [
    {
      path: 'dashboard',
      label: 'Dashboard',
      icon: 'dashboard',
      permission: 'Dashboard.View',
      group: 'Overview',
    },
    {
      path: 'patients',
      label: 'Patients',
      icon: 'patients',
      permission: 'Patients.View',
      group: 'Clinical care',
    },
    {
      path: 'doctors',
      label: 'Doctors',
      icon: 'doctors',
      permission: 'Doctors.View',
      group: 'Clinical care',
    },
    {
      path: 'appointments',
      label: 'Appointments',
      icon: 'appointments',
      permission: 'Appointments.View',
      group: 'Clinical care',
    },
    {
      path: 'admissions',
      label: 'Admissions',
      icon: 'admissions',
      permission: 'Admissions.View',
      group: 'Clinical care',
    },
    {
      path: 'medical-records',
      label: 'Medical records',
      icon: 'medical-records',
      permission: 'MedicalRecords.View',
      group: 'Clinical care',
    },
    {
      path: 'prescriptions',
      label: 'Prescriptions',
      icon: 'prescriptions',
      permission: 'Prescriptions.View',
      group: 'Clinical care',
    },
    {
      path: 'pharmacy',
      label: 'Pharmacy',
      icon: 'pharmacy',
      permission: 'Pharmacy.View',
      group: 'Clinical care',
    },
    {
      path: 'laboratory',
      label: 'Laboratory orders',
      icon: 'laboratory',
      permission: 'Laboratory.View',
      group: 'Diagnostics',
    },
    {
      path: 'lab-tests',
      label: 'Lab test catalog',
      icon: 'lab-tests',
      permission: 'Laboratory.View',
      group: 'Diagnostics',
    },
    {
      path: 'radiology',
      label: 'Radiology orders',
      icon: 'radiology',
      permission: 'Radiology.View',
      group: 'Diagnostics',
    },
    {
      path: 'radiology-tests',
      label: 'Radiology catalog',
      icon: 'radiology-tests',
      permission: 'Radiology.View',
      group: 'Diagnostics',
    },
    {
      path: 'billing',
      label: 'Billing & Invoices',
      icon: 'billing',
      permission: 'Billing.View',
      group: 'Finance & supply',
    },
    {
      path: 'payments',
      label: 'Payments Ledger',
      icon: 'payments',
      permission: 'Payments.View',
      group: 'Finance & supply',
    },
    {
      path: 'insurance',
      label: 'Insurance Claims',
      icon: 'insurance',
      permission: 'Patients.View',
      group: 'Finance & supply',
    },
    {
      path: 'inventory',
      label: 'Inventory Stock',
      icon: 'inventory',
      permission: 'Inventory.View',
      group: 'Finance & supply',
    },
    {
      path: 'purchase-orders',
      label: 'Purchase orders',
      icon: 'purchase-orders',
      permission: 'Inventory.View',
      group: 'Finance & supply',
    },
    {
      path: 'suppliers',
      label: 'Suppliers Directory',
      icon: 'suppliers',
      permission: 'Inventory.View',
      group: 'Finance & supply',
    },
    {
      path: 'staff',
      label: 'Hospital Staff',
      icon: 'staff',
      permission: 'Users.View',
      group: 'Administration',
    },
    {
      path: 'departments',
      label: 'Departments',
      icon: 'departments',
      permission: 'Doctors.View',
      group: 'Administration',
    },
    {
      path: 'users',
      label: 'User Accounts',
      icon: 'users',
      permission: 'Users.View',
      group: 'Administration',
    },
    {
      path: 'roles',
      label: 'Roles & Access',
      icon: 'roles',
      permission: 'Roles.View',
      group: 'Administration',
    },
    {
      path: 'documents',
      label: 'Document Hub',
      icon: 'documents',
      permission: 'Documents.View',
      group: 'Administration',
    },
    {
      path: 'notifications',
      label: 'Notifications',
      icon: 'notifications',
      permission: 'Users.View',
      group: 'Administration',
    },
    {
      path: 'tenants',
      label: 'Tenants & Workspaces',
      icon: 'tenants',
      permission: 'Tenants.Create',
      group: 'Administration',
    },
    {
      path: 'subscriptions',
      label: 'Subscriptions',
      icon: 'subscriptions',
      permission: 'Tenants.View',
      group: 'Administration',
    },
    {
      path: 'audit-logs',
      label: 'Audit Logs',
      icon: 'audit-logs',
      permission: 'Roles.View',
      group: 'Administration',
    },
  ];

  readonly groups = [
    'Overview',
    'Clinical care',
    'Diagnostics',
    'Finance & supply',
    'Administration',
  ] as const;
}
