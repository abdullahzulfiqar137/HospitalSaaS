import { Routes } from '@angular/router';
import { permissionGuard } from '../../core/auth/guards/permission.guard';
export const PATIENT_ROUTES: Routes = [
  {
    path: '',
    canActivate: [permissionGuard],
    data: { permission: 'Patients.View' },
    loadComponent: () =>
      import('./pages/patient-list/patient-list.component').then((m) => m.PatientListComponent),
  },
  {
    path: 'new',
    canActivate: [permissionGuard],
    data: { permission: 'Patients.Create' },
    loadComponent: () =>
      import('./pages/patient-form/patient-form.component').then((m) => m.PatientFormComponent),
  },
  {
    path: ':id/edit',
    canActivate: [permissionGuard],
    data: { permission: 'Patients.Update' },
    loadComponent: () =>
      import('./pages/patient-form/patient-form.component').then((m) => m.PatientFormComponent),
  },
];
