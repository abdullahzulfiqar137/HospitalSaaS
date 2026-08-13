export interface RadiologyOrder {
  id: string;
  patientId: string;
  doctorId: string;
  radiologyTestId: string | null;
  procedure: string;
  orderedAtUtc: string;
  status: string;
  report: string | null;
}
export interface RadiologyTest {
  id: string;
  code: string;
  name: string;
  modality: string;
  price: number;
  isActive: boolean;
}
export interface CreateRadiologyOrderRequest {
  patientId: string;
  doctorId: string;
  procedure: string;
  radiologyTestId: string | null;
}
export interface CreateRadiologyTestRequest {
  code: string;
  name: string;
  modality: string;
  price: number;
}
export interface RadiologyResult {
  id: string;
  report: string;
  version: number;
  recordedAtUtc: string;
}
