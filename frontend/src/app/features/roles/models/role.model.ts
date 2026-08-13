export interface CreateRoleRequest {
  name: string;
  permissions: string[];
}

export interface Role {
  id: string;
  name: string;
  permissions: string[];
}
