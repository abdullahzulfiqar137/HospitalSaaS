export interface Doctor {
  id: string;
  employeeNumber: string;
  firstName: string;
  lastName: string;
  specialization: string;
  departmentId: string;
  licenseNumber: string;
  phone: string | null;
  email: string | null;
  consultationFee: number;
  status: string;
}

export interface CreateDoctorRequest {
  employeeNumber: string;
  firstName: string;
  lastName: string;
  specialization: string;
  departmentId: string;
  licenseNumber: string;
  phone: string | null;
  email: string | null;
  consultationFee: number;
}

export type UpdateDoctorRequest = Omit<CreateDoctorRequest, 'employeeNumber'>;
