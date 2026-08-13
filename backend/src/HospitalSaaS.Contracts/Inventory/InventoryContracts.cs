namespace HospitalSaaS.Contracts.Inventory;

public sealed record CreateInventoryItemRequest(
    string Sku,
    string Name,
    string Unit,
    int MinimumStock
);

public sealed record AdjustInventoryRequest(int Quantity, string Reason);

public sealed record PurchaseOrderItemRequest(Guid InventoryItemId, int Quantity, decimal UnitCost);

public sealed record CreatePurchaseOrderRequest(
    Guid SupplierId,
    DateOnly? ExpectedDeliveryDate,
    string? Notes,
    IReadOnlyList<PurchaseOrderItemRequest> Items
);

public sealed record PurchaseOrderActionRequest(string Action);
