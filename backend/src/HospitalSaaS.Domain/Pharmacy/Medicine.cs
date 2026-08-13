using HospitalSaaS.Domain.Common;

namespace HospitalSaaS.Domain.Pharmacy;

public sealed class Medicine : TenantEntity
{
    private Medicine() { }

    public Medicine(
        Guid tenantId,
        string name,
        string genericName,
        string? brandName,
        string strength,
        string dosageForm,
        string? manufacturer,
        string? barcode,
        decimal salePrice,
        decimal purchasePrice,
        int minimumStockLevel
    )
        : base(tenantId)
    {
        Name = name;
        GenericName = genericName;
        BrandName = brandName;
        Strength = strength;
        DosageForm = dosageForm;
        Manufacturer = manufacturer;
        Barcode = barcode;
        SalePrice = salePrice;
        PurchasePrice = purchasePrice;
        MinimumStockLevel = minimumStockLevel;
        IsActive = true;
    }

    public string Name { get; private set; } = string.Empty;
    public string GenericName { get; private set; } = string.Empty;
    public string? BrandName { get; private set; }
    public string Strength { get; private set; } = string.Empty;
    public string DosageForm { get; private set; } = string.Empty;
    public string? Manufacturer { get; private set; }
    public string? Barcode { get; private set; }
    public decimal SalePrice { get; private set; }
    public decimal PurchasePrice { get; private set; }
    public int MinimumStockLevel { get; private set; }
    public bool IsActive { get; private set; }
}
