namespace HospitalSaaS.Contracts.Inventory;

public sealed record InventoryItemResponse(
    Guid Id,
    string Sku,
    string Name,
    string Unit,
    int MinimumStock,
    bool IsActive
);

public sealed record InventoryTransactionResponse(
    Guid Id,
    Guid InventoryItemId,
    int Quantity,
    string Type,
    string Reason,
    DateTime OccurredAtUtc
);

public sealed record PurchaseOrderItemResponse(
    Guid Id,
    Guid InventoryItemId,
    int Quantity,
    decimal UnitCost,
    decimal Total
);

public sealed record PurchaseOrderResponse(
    Guid Id,
    string OrderNumber,
    Guid SupplierId,
    DateTime OrderedAtUtc,
    DateOnly? ExpectedDeliveryDate,
    string? Notes,
    string Status,
    DateTime? ReceivedAtUtc,
    decimal Total,
    IReadOnlyList<PurchaseOrderItemResponse> Items
);
