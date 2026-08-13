export interface Department {
  id: string;
  name: string;
  code: string;
  description: string | null;
  status: string;
}

export interface CreateDepartmentRequest {
  name: string;
  code: string;
  description: string | null;
}

export interface UpdateDepartmentRequest extends CreateDepartmentRequest {
  headDoctorId: string | null;
}
