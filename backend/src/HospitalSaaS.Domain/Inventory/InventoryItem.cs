using HospitalSaaS.Domain.Common;

namespace HospitalSaaS.Domain.Inventory;

public sealed class InventoryItem : TenantEntity
{
    private InventoryItem() { }

    public InventoryItem(Guid tenantId, string sku, string name, string unit, int minimumStock)
        : base(tenantId)
    {
        Sku = sku;
        Name = name;
        Unit = unit;
        MinimumStock = minimumStock;
        IsActive = true;
    }

    public string Sku { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Unit { get; private set; } = string.Empty;
    public int MinimumStock { get; private set; }
    public bool IsActive { get; private set; }
}
