export interface CreateStaffRequest {
  employeeNumber: string;
  firstName: string;
  lastName: string;
  jobTitle: string;
  departmentId: string | null;
}

export interface Staff extends CreateStaffRequest {
  id: string;
  isActive: boolean;
}
