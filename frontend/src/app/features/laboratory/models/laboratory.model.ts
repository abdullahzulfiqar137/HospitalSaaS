export interface LabOrderItem {
  id: string;
  labTestId: string | null;
  testCode: string;
  testName: string;
  result: string | null;
  unit: string | null;
  referenceRange: string | null;
}
export interface LabOrder {
  id: string;
  patientId: string;
  doctorId: string;
  orderedAtUtc: string;
  status: string;
  items: LabOrderItem[];
}
export interface LabTest {
  id: string;
  name: string;
  code: string;
  category: string;
  price: number;
  normalRange: string | null;
  unit: string | null;
  isActive: boolean;
}
export interface CreateLabTestRequest {
  name: string;
  code: string;
  category: string;
  price: number;
  normalRange: string | null;
  unit: string | null;
}
export interface LabTestRequest {
  code: string;
  name: string;
  labTestId: string | null;
}
export interface CreateLabOrderRequest {
  patientId: string;
  doctorId: string;
  tests: LabTestRequest[];
}
export interface RecordLabResultRequest {
  itemId: string;
  result: string;
  unit: string | null;
  referenceRange: string | null;
}
export interface LabResult {
  id: string;
  labOrderItemId: string;
  value: string;
  unit: string | null;
  referenceRange: string | null;
  version: number;
  recordedAtUtc: string;
}
