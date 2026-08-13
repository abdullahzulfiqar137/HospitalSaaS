import { Routes } from '@angular/router';
import { authGuard } from './core/auth/guards/auth.guard';
import { permissionGuard } from './core/auth/guards/permission.guard';

const modulePage = () =>
  import('./features/module-page/module-page.component').then((m) => m.ModulePageComponent);
const securedModule = (path: string, permission: string) => ({
  path,
  canActivate: [permissionGuard],
  data: { permission },
  loadComponent: modulePage,
});

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./features/authentication/pages/login/login.component').then((m) => m.LoginComponent),
  },
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./layout/main-layout/main-layout.component').then((m) => m.MainLayoutComponent),
    children: [
      {
        path: 'dashboard',
        canActivate: [permissionGuard],
        data: { permission: 'Dashboard.View' },
        loadComponent: () =>
          import('./features/dashboard/pages/dashboard/dashboard.component').then(
            (m) => m.DashboardComponent,
          ),
      },
      {
        path: 'profile',
        loadComponent: () =>
          import('./features/authentication/pages/profile/profile.component').then(
            (m) => m.ProfileComponent,
          ),
      },
      {
        path: 'patients',
        canActivate: [permissionGuard],
        data: { permission: 'Patients.View' },
        loadChildren: () =>
          import('./features/patients/patients.routes').then((m) => m.PATIENT_ROUTES),
      },
      {
        path: 'admissions',
        canActivate: [permissionGuard],
        data: { permission: 'Admissions.View' },
        loadComponent: () =>
          import('./features/admissions/pages/admission-workflow/admission-workflow.component').then(
            (m) => m.AdmissionWorkflowComponent,
          ),
      },
      {
        path: 'purchase-orders',
        canActivate: [permissionGuard],
        data: { permission: 'Inventory.View' },
        loadComponent: () =>
          import('./features/inventory/pages/purchase-orders/purchase-orders.component').then(
            (m) => m.PurchaseOrdersComponent,
          ),
      },
      {
        path: 'documents',
        canActivate: [permissionGuard],
        data: { permission: 'Documents.View' },
        loadComponent: () =>
          import('./features/documents/pages/documents/documents.component').then(
            (m) => m.DocumentsComponent,
          ),
      },
      {
        path: 'doctors',
        canActivate: [permissionGuard],
        data: { permission: 'Doctors.View' },
        loadComponent: () =>
          import('./features/doctors/pages/workforce-workspace/workforce-workspace.component').then(
            (m) => m.WorkforceWorkspaceComponent,
          ),
      },
      {
        path: 'departments',
        canActivate: [permissionGuard],
        data: { permission: 'Doctors.View' },
        loadComponent: () =>
          import('./features/doctors/pages/workforce-workspace/workforce-workspace.component').then(
            (m) => m.WorkforceWorkspaceComponent,
          ),
      },
      {
        path: 'appointments',
        canActivate: [permissionGuard],
        data: { permission: 'Appointments.View' },
        loadComponent: () =>
          import('./features/appointments/pages/appointment-workspace/appointment-workspace.component').then(
            (m) => m.AppointmentWorkspaceComponent,
          ),
      },
      securedModule('wards', 'Admissions.View'),
      securedModule('rooms', 'Admissions.View'),
      securedModule('beds', 'Admissions.View'),
      {
        path: 'medical-records',
        canActivate: [permissionGuard],
        data: { permission: 'MedicalRecords.View' },
        loadComponent: () =>
          import('./features/clinical/pages/clinical-workspace/clinical-workspace.component').then(
            (m) => m.ClinicalWorkspaceComponent,
          ),
      },
      {
        path: 'prescriptions',
        canActivate: [permissionGuard],
        data: { permission: 'Prescriptions.View' },
        loadComponent: () =>
          import('./features/clinical/pages/clinical-workspace/clinical-workspace.component').then(
            (m) => m.ClinicalWorkspaceComponent,
          ),
      },
      {
        path: 'pharmacy',
        canActivate: [permissionGuard],
        data: { permission: 'Pharmacy.View' },
        loadComponent: () =>
          import('./features/pharmacy/pages/pharmacy-workspace/pharmacy-workspace.component').then(
            (m) => m.PharmacyWorkspaceComponent,
          ),
      },
      {
        path: 'laboratory',
        canActivate: [permissionGuard],
        data: { permission: 'Laboratory.View' },
        loadComponent: () =>
          import('./features/diagnostics/pages/diagnostics-workspace/diagnostics-workspace.component').then(
            (m) => m.DiagnosticsWorkspaceComponent,
          ),
      },
      {
        path: 'lab-tests',
        canActivate: [permissionGuard],
        data: { permission: 'Laboratory.View' },
        loadComponent: () =>
          import('./features/diagnostics/pages/diagnostics-workspace/diagnostics-workspace.component').then(
            (m) => m.DiagnosticsWorkspaceComponent,
          ),
      },
      {
        path: 'radiology',
        canActivate: [permissionGuard],
        data: { permission: 'Radiology.View' },
        loadComponent: () =>
          import('./features/diagnostics/pages/diagnostics-workspace/diagnostics-workspace.component').then(
            (m) => m.DiagnosticsWorkspaceComponent,
          ),
      },
      {
        path: 'radiology-tests',
        canActivate: [permissionGuard],
        data: { permission: 'Radiology.View' },
        loadComponent: () =>
          import('./features/diagnostics/pages/diagnostics-workspace/diagnostics-workspace.component').then(
            (m) => m.DiagnosticsWorkspaceComponent,
          ),
      },
      {
        path: 'billing',
        canActivate: [permissionGuard],
        data: { permission: 'Billing.View' },
        loadComponent: () =>
          import('./features/billing/pages/billing-workspace/billing-workspace.component').then(
            (m) => m.BillingWorkspaceComponent,
          ),
      },
      {
        path: 'payments',
        canActivate: [permissionGuard],
        data: { permission: 'Payments.View' },
        loadComponent: () =>
          import('./features/billing/pages/billing-workspace/billing-workspace.component').then(
            (m) => m.BillingWorkspaceComponent,
          ),
      },
      {
        path: 'insurance',
        canActivate: [permissionGuard],
        data: { permission: 'Patients.View' },
        loadComponent: () =>
          import('./features/insurance/pages/insurance-workspace/insurance-workspace.component').then(
            (m) => m.InsuranceWorkspaceComponent,
          ),
      },
      {
        path: 'inventory',
        canActivate: [permissionGuard],
        data: { permission: 'Inventory.View' },
        loadComponent: () =>
          import('./features/inventory/pages/inventory-workspace/inventory-workspace.component').then(
            (m) => m.InventoryWorkspaceComponent,
          ),
      },
      {
        path: 'suppliers',
        canActivate: [permissionGuard],
        data: { permission: 'Inventory.View' },
        loadComponent: () =>
          import('./features/inventory/pages/inventory-workspace/inventory-workspace.component').then(
            (m) => m.InventoryWorkspaceComponent,
          ),
      },
      {
        path: 'staff',
        canActivate: [permissionGuard],
        data: { permission: 'Users.View' },
        loadComponent: () =>
          import('./features/doctors/pages/workforce-workspace/workforce-workspace.component').then(
            (m) => m.WorkforceWorkspaceComponent,
          ),
      },
      {
        path: 'users',
        canActivate: [permissionGuard],
        data: { permission: 'Users.View' },
        loadComponent: () =>
          import('./features/administration/pages/access-workspace/access-workspace.component').then(
            (m) => m.AccessWorkspaceComponent,
          ),
      },
      {
        path: 'roles',
        canActivate: [permissionGuard],
        data: { permission: 'Roles.View' },
        loadComponent: () =>
          import('./features/administration/pages/access-workspace/access-workspace.component').then(
            (m) => m.AccessWorkspaceComponent,
          ),
      },
      securedModule('permissions', 'Roles.View'),
      securedModule('tenants', 'Tenants.Create'),
      securedModule('notifications', 'Users.View'),
      {
        path: 'subscriptions',
        canActivate: [permissionGuard],
        data: { permission: 'Tenants.View' },
        loadComponent: () =>
          import('./features/subscriptions/pages/subscriptions-workspace/subscriptions-workspace.component').then(
            (m) => m.SubscriptionsWorkspaceComponent,
          ),
      },
      securedModule('audit-logs', 'Roles.View'),
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
    ],
  },
  { path: '**', redirectTo: '' },
];
