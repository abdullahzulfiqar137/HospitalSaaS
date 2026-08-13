export interface CreateTenantRequest {
  name: string;
  slug: string;
}

export interface Tenant extends CreateTenantRequest {
  id: string;
  isActive: boolean;
}
