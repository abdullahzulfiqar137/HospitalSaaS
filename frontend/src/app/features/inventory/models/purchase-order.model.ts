export interface PurchaseOrderItem {
  inventoryItemId: string;
  quantity: number;
  unitCost: number;
}
export interface PurchaseOrder {
  id: string;
  orderNumber: string;
  supplierId: string;
  orderedAtUtc: string;
  expectedDeliveryDate: string | null;
  notes: string | null;
  status: string;
  receivedAtUtc: string | null;
  total: number;
  items: Array<PurchaseOrderItem & { id: string; total: number }>;
}
export interface CreatePurchaseOrder {
  supplierId: string;
  expectedDeliveryDate: string | null;
  notes: string | null;
  items: PurchaseOrderItem[];
}

export type PurchaseOrderAction = 'Cancel' | 'Receive';

export interface PurchaseOrderQuery {
  status?: string;
  pageNumber?: number;
  pageSize?: number;
}
