export interface CreateSupplierRequest {
  name: string;
  contactName: string | null;
  phone: string | null;
  email: string | null;
  address: string | null;
}

export interface Supplier extends CreateSupplierRequest {
  id: string;
  isActive: boolean;
}
