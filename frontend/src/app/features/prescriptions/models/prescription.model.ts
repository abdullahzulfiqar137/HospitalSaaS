export interface PrescriptionLine {
  id: string;
  medicineId: string;
  dose: string;
  frequency: string;
  duration: string;
  route: string;
  instructions: string | null;
}
export interface Prescription {
  id: string;
  patientId: string;
  doctorId: string;
  appointmentId: string | null;
  dateUtc: string;
  notes: string | null;
  items: PrescriptionLine[];
}
export type PrescriptionItemRequest = Omit<PrescriptionLine, 'id'>;
export interface CreatePrescriptionRequest {
  patientId: string;
  doctorId: string;
  appointmentId: string | null;
  notes: string | null;
  items: PrescriptionItemRequest[];
}
