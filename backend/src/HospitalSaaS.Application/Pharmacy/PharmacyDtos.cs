using HospitalSaaS.Domain.Pharmacy;

namespace HospitalSaaS.Application.Pharmacy;

public sealed record MedicineDto(
    Guid Id,
    string Name,
    string GenericName,
    string? BrandName,
    string Strength,
    string DosageForm,
    string? Manufacturer,
    string? Barcode,
    decimal SalePrice,
    decimal PurchasePrice,
    int MinimumStockLevel,
    bool IsActive
)
{
    public static MedicineDto From(Medicine x) =>
        new(
            x.Id,
            x.Name,
            x.GenericName,
            x.BrandName,
            x.Strength,
            x.DosageForm,
            x.Manufacturer,
            x.Barcode,
            x.SalePrice,
            x.PurchasePrice,
            x.MinimumStockLevel,
            x.IsActive
        );
}

public sealed record StockTransactionDto(
    Guid Id,
    Guid MedicineId,
    StockTransactionType Type,
    int Quantity,
    string? Reference,
    DateTime OccurredAtUtc
)
{
    public static StockTransactionDto From(StockTransaction x) =>
        new(x.Id, x.MedicineId, x.Type, x.Quantity, x.Reference, x.OccurredAtUtc);
}
