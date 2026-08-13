export interface CreateUserRequest {
  email: string;
  password: string;
  roleIds: string[];
}

export interface User {
  id: string;
  email: string;
  isActive: boolean;
  roles: string[];
}
