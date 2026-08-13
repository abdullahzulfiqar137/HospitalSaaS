export interface CreatePatientRequest {
  firstName: string;
  middleName: string | null;
  lastName: string;
  dateOfBirth: string;
  gender: string;
  phone: string | null;
  email: string | null;
  nationalId: string | null;
  address: string | null;
  emergencyContactName: string | null;
  emergencyContactPhone: string | null;
  maritalStatus: string | null;
  notes: string | null;
}
export interface UpdatePatientRequest extends CreatePatientRequest {
  bloodGroup: string | null;
}
