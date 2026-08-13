using HospitalSaaS.Domain.Common;

namespace HospitalSaaS.Domain.Inventory;

public sealed class PurchaseOrderItem : BaseEntity
{
    private PurchaseOrderItem() { }

    internal PurchaseOrderItem(
        Guid purchaseOrderId,
        Guid inventoryItemId,
        int quantity,
        decimal unitCost
    )
    {
        PurchaseOrderId = purchaseOrderId;
        InventoryItemId = inventoryItemId;
        Quantity = quantity;
        UnitCost = unitCost;
    }

    public Guid PurchaseOrderId { get; private set; }
    public Guid InventoryItemId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitCost { get; private set; }
    public decimal Total => Quantity * UnitCost;
}
