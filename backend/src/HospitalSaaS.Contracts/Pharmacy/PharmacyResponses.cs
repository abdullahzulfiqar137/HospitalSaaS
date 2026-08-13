namespace HospitalSaaS.Contracts.Pharmacy;

public sealed record MedicineResponse(
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
);

public sealed record StockTransactionResponse(
    Guid Id,
    Guid MedicineId,
    string Type,
    int Quantity,
    string? Reference,
    DateTime OccurredAtUtc
);
