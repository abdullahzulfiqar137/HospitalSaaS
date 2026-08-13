export interface CreateInventoryItemRequest {
  sku: string;
  name: string;
  unit: string;
  minimumStock: number;
}

export interface InventoryItem {
  id: string;
  sku: string;
  name: string;
  unit: string;
  minimumStock: number;
  isActive: boolean;
}

export interface AdjustInventoryRequest {
  quantity: number;
  reason: string;
}

export interface InventoryTransaction {
  id: string;
  inventoryItemId: string;
  quantity: number;
  type: string;
  reason: string;
  occurredAtUtc: string;
}

export interface InventoryStock {
  quantity: number;
}
