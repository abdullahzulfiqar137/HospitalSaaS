export interface Diagnosis {
  id: string;
  code: string;
  description: string;
  isPrimary: boolean;
}
export interface DiagnosisRequest {
  code: string;
  description: string;
  isPrimary: boolean;
}
export interface MedicalRecord {
  id: string;
  patientId: string;
  doctorId: string;
  appointmentId: string | null;
  chiefComplaint: string;
  history: string | null;
  examination: string | null;
  clinicalNotes: string | null;
  treatmentPlan: string | null;
  followUpDate: string | null;
  diagnoses: Diagnosis[];
}
export interface CreateMedicalRecordRequest {
  patientId: string;
  doctorId: string;
  appointmentId: string | null;
  chiefComplaint: string;
  history: string | null;
  examination: string | null;
  clinicalNotes: string | null;
  treatmentPlan: string | null;
  followUpDate: string | null;
  diagnoses: DiagnosisRequest[] | null;
}
