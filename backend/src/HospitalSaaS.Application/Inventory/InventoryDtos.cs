using HospitalSaaS.Domain.Inventory;

namespace HospitalSaaS.Application.Inventory;

public sealed record InventoryItemDto(
    Guid Id,
    string Sku,
    string Name,
    string Unit,
    int MinimumStock,
    bool IsActive
)
{
    public static InventoryItemDto From(InventoryItem x) =>
        new(x.Id, x.Sku, x.Name, x.Unit, x.MinimumStock, x.IsActive);
}

public sealed record PurchaseOrderItemDto(
    Guid Id,
    Guid InventoryItemId,
    int Quantity,
    decimal UnitCost,
    decimal Total
);

public sealed record PurchaseOrderDto(
    Guid Id,
    string OrderNumber,
    Guid SupplierId,
    DateTime OrderedAtUtc,
    DateOnly? ExpectedDeliveryDate,
    string? Notes,
    PurchaseOrderStatus Status,
    DateTime? ReceivedAtUtc,
    decimal Total,
    IReadOnlyList<PurchaseOrderItemDto> Items
)
{
    public static PurchaseOrderDto From(PurchaseOrder order) =>
        new(
            order.Id,
            order.OrderNumber,
            order.SupplierId,
            order.OrderedAtUtc,
            order.ExpectedDeliveryDate,
            order.Notes,
            order.Status,
            order.ReceivedAtUtc,
            order.Total,
            order
                .Items.Select(x => new PurchaseOrderItemDto(
                    x.Id,
                    x.InventoryItemId,
                    x.Quantity,
                    x.UnitCost,
                    x.Total
                ))
                .ToList()
        );
}

public sealed record InventoryTransactionDto(
    Guid Id,
    Guid InventoryItemId,
    int Quantity,
    string Type,
    string Reason,
    DateTime OccurredAtUtc
)
{
    public static InventoryTransactionDto From(InventoryTransaction x) =>
        new(x.Id, x.InventoryItemId, x.Quantity, x.Type, x.Reason, x.OccurredAtUtc);
}
