export interface Patient {
  id: string;
  patientNumber: string;
  firstName: string;
  middleName: string | null;
  lastName: string;
  dateOfBirth: string;
  gender: string;
  bloodGroup: string | null;
  phone: string | null;
  email: string | null;
  nationalId: string | null;
  address: string | null;
  emergencyContactName: string | null;
  emergencyContactPhone: string | null;
  maritalStatus: string | null;
  registrationDateUtc: string;
  notes: string | null;
  status: string;
}
