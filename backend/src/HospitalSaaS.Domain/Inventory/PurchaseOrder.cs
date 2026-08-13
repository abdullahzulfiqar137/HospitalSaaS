using HospitalSaaS.Domain.Common;
using HospitalSaaS.Domain.Exceptions;

namespace HospitalSaaS.Domain.Inventory;

public sealed class PurchaseOrder : AggregateRoot
{
    private readonly List<PurchaseOrderItem> _items = [];

    private PurchaseOrder() { }

    public PurchaseOrder(
        Guid tenantId,
        string orderNumber,
        Guid supplierId,
        DateOnly? expectedDeliveryDate,
        string? notes
    )
        : base(tenantId)
    {
        OrderNumber = orderNumber;
        SupplierId = supplierId;
        OrderedAtUtc = DateTime.UtcNow;
        ExpectedDeliveryDate = expectedDeliveryDate;
        Notes = notes?.Trim();
        Status = PurchaseOrderStatus.Draft;
    }

    public string OrderNumber { get; private set; } = string.Empty;
    public Guid SupplierId { get; private set; }
    public DateTime OrderedAtUtc { get; private set; }
    public DateOnly? ExpectedDeliveryDate { get; private set; }
    public string? Notes { get; private set; }
    public PurchaseOrderStatus Status { get; private set; }
    public DateTime? ReceivedAtUtc { get; private set; }
    public IReadOnlyCollection<PurchaseOrderItem> Items => _items;
    public decimal Total => _items.Sum(x => x.Total);

    public void AddItem(Guid inventoryItemId, int quantity, decimal unitCost)
    {
        if (Status != PurchaseOrderStatus.Draft)
            throw new DomainException("Only draft purchase orders can be changed.");
        if (quantity <= 0 || unitCost < 0)
            throw new DomainException("Purchase order quantity and cost are invalid.");
        if (_items.Any(x => x.InventoryItemId == inventoryItemId))
            throw new DomainException("An inventory item can appear only once per purchase order.");
        _items.Add(new PurchaseOrderItem(Id, inventoryItemId, quantity, unitCost));
    }

    public void Submit()
    {
        if (Status != PurchaseOrderStatus.Draft || _items.Count == 0)
            throw new DomainException("A non-empty draft purchase order is required.");
        Status = PurchaseOrderStatus.Submitted;
    }

    public void Receive()
    {
        if (Status != PurchaseOrderStatus.Submitted)
            throw new DomainException("Only submitted purchase orders can be received.");
        Status = PurchaseOrderStatus.Received;
        ReceivedAtUtc = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status is PurchaseOrderStatus.Received or PurchaseOrderStatus.Cancelled)
            throw new DomainException("The purchase order cannot be cancelled.");
        Status = PurchaseOrderStatus.Cancelled;
    }
}

public enum PurchaseOrderStatus
{
    Draft,
    Submitted,
    Received,
    Cancelled,
}
