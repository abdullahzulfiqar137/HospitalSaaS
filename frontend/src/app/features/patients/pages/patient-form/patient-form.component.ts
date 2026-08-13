import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { NotificationService } from '../../../../core/http/notification.service';
import { CreatePatientRequest, UpdatePatientRequest } from '../../models/patient-request.model';
import { PatientService } from '../../services/patient.service';
import { Country, CountrySelectorComponent, COUNTRIES, formatNationalId, formatPhoneNumber } from '../../../../shared/components/country-selector/country-selector.component';

@Component({
  selector: 'app-patient-form',
  imports: [ReactiveFormsModule, RouterLink, CountrySelectorComponent],
  templateUrl: './patient-form.component.html',
  styleUrl: './patient-form.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PatientFormComponent {
  private readonly service = inject(PatientService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly notices = inject(NotificationService);
  readonly id = this.route.snapshot.paramMap.get('id');
  readonly saving = signal(false);
  readonly loading = signal(!!this.id);

  readonly selectedIdCountry = signal<Country>(COUNTRIES[0]);
  readonly selectedPhoneCountry = signal<Country>(COUNTRIES[0]);
  readonly selectedEmergencyPhoneCountry = signal<Country>(COUNTRIES[0]);

  readonly nationalIdPlaceholder = signal('e.g. 42101-1234567-1 (CNIC)');
  readonly nationalIdMaxLength = signal(15);
  readonly phonePlaceholder = signal('300 1234567');
  readonly phoneMaxLength = signal(11);
  readonly emergencyPhonePlaceholder = signal('300 1234567');
  readonly emergencyPhoneMaxLength = signal(11);

  onIdCountryChange(country: Country): void {
    this.selectedIdCountry.set(country);
    this.nationalIdPlaceholder.set(`${country.idExample} (${country.idCardName})`);
    this.nationalIdMaxLength.set(country.idMaxLength);
    const current = this.form.controls.nationalId.value || '';
    this.form.controls.nationalId.setValue(formatNationalId(country.iso, current));
  }

  onPhoneCountryChange(country: Country): void {
    this.selectedPhoneCountry.set(country);
    this.phonePlaceholder.set(country.phoneExample);
    this.phoneMaxLength.set(country.phoneMaxLength);
    const current = this.form.controls.phone.value || '';
    this.form.controls.phone.setValue(formatPhoneNumber(country.iso, current));
  }

  onEmergencyPhoneCountryChange(country: Country): void {
    this.selectedEmergencyPhoneCountry.set(country);
    this.emergencyPhonePlaceholder.set(country.phoneExample);
    this.emergencyPhoneMaxLength.set(country.phoneMaxLength);
    const current = this.form.controls.emergencyContactPhone.value || '';
    this.form.controls.emergencyContactPhone.setValue(formatPhoneNumber(country.iso, current));
  }

  onNationalIdInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    const formatted = formatNationalId(this.selectedIdCountry().iso, input.value);
    this.form.controls.nationalId.setValue(formatted, { emitEvent: false });
    input.value = formatted;
  }

  onPhoneInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    const formatted = formatPhoneNumber(this.selectedPhoneCountry().iso, input.value);
    this.form.controls.phone.setValue(formatted, { emitEvent: false });
    input.value = formatted;
  }

  onEmergencyPhoneInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    const formatted = formatPhoneNumber(this.selectedEmergencyPhoneCountry().iso, input.value);
    this.form.controls.emergencyContactPhone.setValue(formatted, { emitEvent: false });
    input.value = formatted;
  }
  readonly form = new FormGroup({
    firstName: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(100)],
    }),
    middleName: new FormControl<string | null>(null),
    lastName: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(100)],
    }),
    dateOfBirth: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    gender: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    bloodGroup: new FormControl<string | null>(null),
    phone: new FormControl<string | null>(null),
    email: new FormControl<string | null>(null, [Validators.email]),
    nationalId: new FormControl<string | null>(null),
    address: new FormControl<string | null>(null),
    emergencyContactName: new FormControl<string | null>(null),
    emergencyContactPhone: new FormControl<string | null>(null),
    maritalStatus: new FormControl<string | null>(null),
    notes: new FormControl<string | null>(null),
  });
  constructor() {
    if (this.id)
      this.service
        .get(this.id)
        .pipe(finalize(() => this.loading.set(false)))
        .subscribe((p) => this.form.patchValue(p));
  }
  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const value = this.form.getRawValue();
    const request = this.id
      ? this.service.update(this.id, value as UpdatePatientRequest)
      : this.service.create(value as CreatePatientRequest);
    request.pipe(finalize(() => this.saving.set(false))).subscribe(() => {
      this.notices.show(`Patient ${this.id ? 'updated' : 'created'} successfully.`, 'success');
      void this.router.navigate(['/patients']);
    });
  }
}
