namespace HospitalSaaS.Contracts.Pharmacy;

public sealed record CreateMedicineRequest(
    string Name,
    string GenericName,
    string? BrandName,
    string Strength,
    string DosageForm,
    string? Manufacturer,
    string? Barcode,
    decimal SalePrice,
    decimal PurchasePrice,
    int MinimumStockLevel
);

public sealed record StockRequest(string Type, int Quantity, string? Reference);
