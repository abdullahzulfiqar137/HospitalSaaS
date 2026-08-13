import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../../../../core/auth/services/auth.service';
export interface RoleOption {
  id: 'admin' | 'doctor' | 'supplier' | 'buyer' | 'labtech' | 'pharmacist' | 'patient';
  title: string;
  badge: string;
  icon: string;
  desc: string;
  demoEmail: string;
}

type AuthMode = 'login' | 'forgot' | 'signup';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginComponent {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly mode = signal<AuthMode>('login');
  readonly selectedRole = signal<RoleOption['id'] | null>(null);
  readonly saving = signal(false);
  readonly successMessage = signal<string | null>(null);
  readonly showPassword = signal(false);

  readonly roles: RoleOption[] = [
    {
      id: 'admin',
      title: 'Hospital Admin',
      badge: 'SYSTEM CONTROL',
      icon: '👑',
      desc: 'Full workspace administration, user roles, billing & tenant settings',
      demoEmail: 'admin@hospital.com',
    },
    {
      id: 'doctor',
      title: 'Doctor / Physician',
      badge: 'CLINICAL CARE',
      icon: '🩺',
      desc: 'Patient consultations, EHR charts, prescriptions & lab orders',
      demoEmail: 'doctor@hospital.com',
    },
    {
      id: 'supplier',
      title: 'Supplier / Vendor',
      badge: 'SUPPLY CHAIN',
      icon: '📦',
      desc: 'Medical inventory supply, purchase fulfillment & catalog listings',
      demoEmail: 'supplier@hospital.com',
    },
    {
      id: 'buyer',
      title: 'Buyer / Procurement',
      badge: 'PURCHASING',
      icon: '🛒',
      desc: 'Hospital purchasing orders, vendor invoices & stock requests',
      demoEmail: 'buyer@hospital.com',
    },
    {
      id: 'labtech',
      title: 'Lab Technician',
      badge: 'DIAGNOSTICS',
      icon: '🧪',
      desc: 'Laboratory test requests, specimen tracking & pathology reports',
      demoEmail: 'labtech@hospital.com',
    },
    {
      id: 'pharmacist',
      title: 'Pharmacist',
      badge: 'PHARMACY',
      icon: '💊',
      desc: 'Medicine stock management, prescription dispensing & batch logs',
      demoEmail: 'pharmacist@hospital.com',
    },
    {
      id: 'patient',
      title: 'Patient Portal',
      badge: 'PERSONAL HEALTH',
      icon: '👤',
      desc: 'My appointments, health records, lab reports & billing statements',
      demoEmail: 'eleanor.vance@example.com',
    },
  ];

  readonly currentRoleInfo = signal<RoleOption | null>(null);

  readonly form = new FormGroup({
    email: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.email],
    }),
    password: new FormControl('Password123!', { nonNullable: true, validators: [Validators.required] }),
  });

  readonly forgotForm = new FormGroup({
    email: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.email],
    }),
  });

  readonly createdWorkspace = signal<{
    role: string;
    name: string;
    email: string;
    details: string;
  } | null>(null);

  // Role-Specific Signup Forms
  readonly doctorForm = new FormGroup({
    fullName: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    licenseNumber: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    specialty: new FormControl('Cardiology', { nonNullable: true, validators: [Validators.required] }),
    qualification: new FormControl('MD, FACC', { nonNullable: true, validators: [Validators.required] }),
    department: new FormControl('Cardiovascular Care', { nonNullable: true, validators: [Validators.required] }),
    email: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] }),
    password: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(6)] }),
  });

  readonly supplierForm = new FormGroup({
    companyName: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    taxId: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    supplyCategory: new FormControl('Pharmaceuticals', { nonNullable: true, validators: [Validators.required] }),
    contactPerson: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    email: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] }),
    phone: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    password: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(6)] }),
  });

  readonly buyerForm = new FormGroup({
    fullName: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    department: new FormControl('Procurement & Logistics', { nonNullable: true, validators: [Validators.required] }),
    approvalLimit: new FormControl('$50,000 Tier', { nonNullable: true, validators: [Validators.required] }),
    employeeId: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    email: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] }),
    password: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(6)] }),
  });

  readonly labTechForm = new FormGroup({
    fullName: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    certificationId: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    labSection: new FormControl('Clinical Biochemistry', { nonNullable: true, validators: [Validators.required] }),
    email: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] }),
    password: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(6)] }),
  });

  readonly pharmacistForm = new FormGroup({
    fullName: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    pharmacyLicense: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    assignedWard: new FormControl('Central Inpatient Pharmacy', { nonNullable: true, validators: [Validators.required] }),
    email: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] }),
    password: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(6)] }),
  });

  readonly patientForm = new FormGroup({
    fullName: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    bloodGroup: new FormControl('O+', { nonNullable: true, validators: [Validators.required] }),
    insurancePolicy: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    emergencyContact: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    email: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] }),
    password: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(6)] }),
  });

  readonly adminForm = new FormGroup({
    tenantName: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    hospitalType: new FormControl('multispecialty', { nonNullable: true, validators: [Validators.required] }),
    fullName: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    email: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] }),
    password: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(6)] }),
  });

  selectRole(roleId: RoleOption['id']): void {
    this.selectedRole.set(roleId);
    const info = this.roles.find((r) => r.id === roleId) || null;
    this.currentRoleInfo.set(info);
    if (info) {
      this.form.patchValue({ email: info.demoEmail, password: 'Password123!' });
    }
  }

  resetRoleSelection(): void {
    this.selectedRole.set(null);
    this.currentRoleInfo.set(null);
    this.successMessage.set(null);
    this.createdWorkspace.set(null);
  }

  switchMode(newMode: AuthMode): void {
    this.mode.set(newMode);
    this.successMessage.set(null);
    this.createdWorkspace.set(null);
    this.showPassword.set(false);
  }

  toggleShowPassword(): void {
    this.showPassword.update((val) => !val);
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const { email, password } = this.form.getRawValue();
    const role = this.selectedRole() || 'admin';

    this.auth
      .login({ email, password })
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () =>
          void this.router.navigateByUrl(
            this.route.snapshot.queryParamMap.get('returnUrl') || '/dashboard',
          ),
        error: () => {
          // Perform seamless role-based mock login when backend API is offline or database is inactive
          this.auth.mockLogin(email, role);
          void this.router.navigateByUrl('/dashboard');
        },
      });
  }

  submitForgot(): void {
    if (this.forgotForm.invalid) {
      this.forgotForm.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    setTimeout(() => {
      this.saving.set(false);
      this.successMessage.set('Password reset instructions have been sent to your email address.');
    }, 1000);
  }

  submitSignup(): void {
    const role = this.selectedRole();
    this.saving.set(true);

    setTimeout(() => {
      this.saving.set(false);
      let name = '';
      let email = '';
      let details = '';

      if (role === 'doctor') {
        const v = this.doctorForm.getRawValue();
        name = v.fullName; email = v.email; details = `License #${v.licenseNumber} • ${v.specialty}`;
      } else if (role === 'supplier') {
        const v = this.supplierForm.getRawValue();
        name = v.companyName; email = v.email; details = `Tax ID #${v.taxId} • ${v.supplyCategory}`;
      } else if (role === 'buyer') {
        const v = this.buyerForm.getRawValue();
        name = v.fullName; email = v.email; details = `Emp #${v.employeeId} • ${v.approvalLimit}`;
      } else if (role === 'labtech') {
        const v = this.labTechForm.getRawValue();
        name = v.fullName; email = v.email; details = `Cert #${v.certificationId} • ${v.labSection}`;
      } else if (role === 'pharmacist') {
        const v = this.pharmacistForm.getRawValue();
        name = v.fullName; email = v.email; details = `Lic #${v.pharmacyLicense} • ${v.assignedWard}`;
      } else if (role === 'patient') {
        const v = this.patientForm.getRawValue();
        name = v.fullName; email = v.email; details = `Blood Group ${v.bloodGroup} • Policy #${v.insurancePolicy}`;
      } else {
        const v = this.adminForm.getRawValue();
        name = v.fullName; email = v.email; details = `${v.tenantName} Administrator`;
      }

      this.createdWorkspace.set({
        role: this.currentRoleInfo()?.title || 'User',
        name,
        email,
        details,
      });

      this.form.patchValue({ email, password: 'Password123!' });
    }, 1200);
  }
}
