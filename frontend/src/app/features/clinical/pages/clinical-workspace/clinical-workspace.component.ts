import { DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormArray, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize, forkJoin, Observable } from 'rxjs';
import { AuthService } from '../../../../core/auth/services/auth.service';
import { NotificationService } from '../../../../core/http/notification.service';
import { Doctor } from '../../../doctors/models/doctor.model';
import { DoctorService } from '../../../doctors/services/doctor.service';
import { MedicalRecord } from '../../../medical-records/models/medical-record.model';
import { MedicalRecordService } from '../../../medical-records/services/medical-record.service';
import { Patient } from '../../../patients/models/patient.model';
import { PatientService } from '../../../patients/services/patient.service';
import { Medicine } from '../../../pharmacy/models/pharmacy.model';
import { PharmacyService } from '../../../pharmacy/services/pharmacy.service';
import { Prescription } from '../../../prescriptions/models/prescription.model';
import { PrescriptionService } from '../../../prescriptions/services/prescription.service';

@Component({
  selector: 'app-clinical-workspace',
  imports: [ReactiveFormsModule, DatePipe],
  templateUrl: './clinical-workspace.component.html',
  styleUrl: './clinical-workspace.component.scss',
})
export class ClinicalWorkspaceComponent {
  private readonly router = inject(Router);
  private readonly recordsApi = inject(MedicalRecordService);
  private readonly prescriptionsApi = inject(PrescriptionService);
  private readonly patientsApi = inject(PatientService);
  private readonly doctorsApi = inject(DoctorService);
  private readonly pharmacyApi = inject(PharmacyService);
  private readonly notices = inject(NotificationService);
  readonly auth = inject(AuthService);
  readonly mode = signal<'records' | 'prescriptions'>(
    this.router.url.includes('prescriptions') ? 'prescriptions' : 'records',
  );
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly modal = signal(false);
  readonly patients = signal<Patient[]>([]);
  readonly doctors = signal<Doctor[]>([]);
  readonly medicines = signal<Medicine[]>([]);
  readonly records = signal<MedicalRecord[]>([]);
  readonly prescriptions = signal<Prescription[]>([]);
  readonly patientFilter = signal('');

  readonly recordForm = new FormGroup({
    patientId: new FormControl('', { nonNullable: true, validators: Validators.required }),
    doctorId: new FormControl('', { nonNullable: true, validators: Validators.required }),
    appointmentId: new FormControl<string | null>(null),
    chiefComplaint: new FormControl('', { nonNullable: true, validators: Validators.required }),
    history: new FormControl<string | null>(null),
    examination: new FormControl<string | null>(null),
    clinicalNotes: new FormControl<string | null>(null),
    treatmentPlan: new FormControl<string | null>(null),
    followUpDate: new FormControl<string | null>(null),
    diagnoses: new FormArray([this.diagnosisGroup()]),
  });
  readonly prescriptionForm = new FormGroup({
    patientId: new FormControl('', { nonNullable: true, validators: Validators.required }),
    doctorId: new FormControl('', { nonNullable: true, validators: Validators.required }),
    appointmentId: new FormControl<string | null>(null),
    notes: new FormControl<string | null>(null),
    items: new FormArray([this.medicineGroup()]),
  });

  constructor() {
    this.load();
  }
  get diagnoses(): FormArray {
    return this.recordForm.controls.diagnoses;
  }
  get items(): FormArray {
    return this.prescriptionForm.controls.items;
  }

  load(): void {
    this.loading.set(true);
    forkJoin({
      records: this.recordsApi.search(this.patientFilter() || undefined, 1, 100),
      prescriptions: this.prescriptionsApi.search(this.patientFilter() || undefined, 1, 100),
      patients: this.patientsApi.list({ pageSize: 100 }),
      doctors: this.doctorsApi.list({ pageSize: 100 }),
      medicines: this.pharmacyApi.list({ pageSize: 100 }),
    })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe((data) => {
        this.records.set(data.records.items);
        this.prescriptions.set(data.prescriptions.items);
        this.patients.set(data.patients.items);
        this.doctors.set(data.doctors.items);
        this.medicines.set(data.medicines.items);
      });
  }
  addDiagnosis(): void {
    this.diagnoses.push(this.diagnosisGroup());
  }
  addMedicine(): void {
    this.items.push(this.medicineGroup());
  }
  removeDiagnosis(index: number): void {
    if (this.diagnoses.length > 1) this.diagnoses.removeAt(index);
  }
  removeMedicine(index: number): void {
    if (this.items.length > 1) this.items.removeAt(index);
  }
  submit(): void {
    const form = this.mode() === 'records' ? this.recordForm : this.prescriptionForm;
    if (form.invalid) return form.markAllAsTouched();
    this.saving.set(true);
    const request: Observable<unknown> =
      this.mode() === 'records'
        ? this.recordsApi.createRecord(this.recordForm.getRawValue())
        : this.prescriptionsApi.createPrescription(this.prescriptionForm.getRawValue());
    request.pipe(finalize(() => this.saving.set(false))).subscribe(() => {
      this.notices.show(
        this.mode() === 'records' ? 'Medical record created.' : 'Prescription created.',
        'success',
      );
      this.modal.set(false);
      this.reset();
      this.load();
    });
  }
  deleteRecord(record: MedicalRecord): void {
    if (!confirm('Delete this medical record?')) return;
    this.recordsApi.delete(record.id).subscribe(() => {
      this.notices.show('Medical record deleted.', 'success');
      this.load();
    });
  }
  formatPatientLabel(patient: { firstName: string; lastName: string; patientNumber: string }): string {
    const cleanLast = (patient.lastName || '').replace(/\d+/g, '').trim();
    const name = `${patient.firstName} ${cleanLast}`.trim();
    return `${name} (${patient.patientNumber})`;
  }

  patientName(id: string): string {
    const item = this.patients().find((patient) => patient.id === id);
    return item ? `${item.firstName} ${item.lastName}` : id;
  }
  doctorName(id: string): string {
    const item = this.doctors().find((doctor) => doctor.id === id);
    return item ? `Dr. ${item.firstName} ${item.lastName}` : id;
  }
  medicineName(id: string): string {
    return this.medicines().find((medicine) => medicine.id === id)?.name ?? id;
  }
  private diagnosisGroup() {
    return new FormGroup({
      code: new FormControl('', { nonNullable: true, validators: Validators.required }),
      description: new FormControl('', { nonNullable: true, validators: Validators.required }),
      isPrimary: new FormControl(false, { nonNullable: true }),
    });
  }
  private medicineGroup() {
    return new FormGroup({
      medicineId: new FormControl('', { nonNullable: true, validators: Validators.required }),
      dose: new FormControl('', { nonNullable: true, validators: Validators.required }),
      frequency: new FormControl('', { nonNullable: true, validators: Validators.required }),
      duration: new FormControl('', { nonNullable: true, validators: Validators.required }),
      route: new FormControl('Oral', { nonNullable: true, validators: Validators.required }),
      instructions: new FormControl<string | null>(null),
    });
  }
  private reset(): void {
    while (this.diagnoses.length > 1) this.diagnoses.removeAt(1);
    while (this.items.length > 1) this.items.removeAt(1);
    this.recordForm.reset();
    this.prescriptionForm.reset();
  }
}
