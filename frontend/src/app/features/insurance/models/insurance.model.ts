export interface CreateInsurancePolicyRequest {
  patientId: string;
  provider: string;
  policyNumber: string;
  validFrom: string;
  validTo: string;
}

export interface InsurancePolicy extends CreateInsurancePolicyRequest {
  id: string;
}

export interface InsuranceQuery {
  patientId?: string;
  pageNumber?: number;
  pageSize?: number;
}
