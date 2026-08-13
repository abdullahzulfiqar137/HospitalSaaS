export interface Medicine {
  id: string;
  name: string;
  genericName: string;
  brandName: string | null;
  strength: string;
  dosageForm: string;
  manufacturer: string | null;
  barcode: string | null;
  salePrice: number;
  purchasePrice: number;
  minimumStockLevel: number;
  isActive: boolean;
}
export interface CreateMedicineRequest {
  name: string;
  genericName: string;
  brandName: string | null;
  strength: string;
  dosageForm: string;
  manufacturer: string | null;
  barcode: string | null;
  salePrice: number;
  purchasePrice: number;
  minimumStockLevel: number;
}
export interface StockRequest {
  type: 'Purchase' | 'Sale' | 'Adjustment' | 'Return';
  quantity: number;
  reference: string | null;
}
export interface StockTransaction {
  id: string;
  medicineId: string;
  type: string;
  quantity: number;
  reference: string | null;
  occurredAtUtc: string;
}
export interface StockBalance {
  quantity: number;
}
