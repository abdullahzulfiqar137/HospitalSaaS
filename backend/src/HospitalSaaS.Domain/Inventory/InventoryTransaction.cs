using HospitalSaaS.Domain.Common;
using HospitalSaaS.Domain.Exceptions;

namespace HospitalSaaS.Domain.Inventory;

public sealed class InventoryTransaction : TenantEntity
{
    private InventoryTransaction() { }

    public InventoryTransaction(
        Guid tenantId,
        Guid itemId,
        int quantity,
        string type,
        string reason
    )
        : base(tenantId)
    {
        if (quantity == 0)
            throw new DomainException("Quantity cannot be zero.");
        InventoryItemId = itemId;
        Quantity = quantity;
        Type = type;
        Reason = reason;
        OccurredAtUtc = DateTime.UtcNow;
    }

    public Guid InventoryItemId { get; private set; }
    public int Quantity { get; private set; }
    public string Type { get; private set; } = string.Empty;
    public string Reason { get; private set; } = string.Empty;
    public DateTime OccurredAtUtc { get; private set; }
}
