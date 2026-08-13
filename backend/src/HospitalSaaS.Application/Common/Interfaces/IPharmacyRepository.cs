using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Pharmacy;

namespace HospitalSaaS.Application.Common.Interfaces;

public interface IPharmacyRepository
{
    Task AddMedicineAsync(Medicine medicine, CancellationToken ct);
    Task<Medicine?> GetMedicineAsync(Guid id, bool tracking, CancellationToken ct);
    Task<PagedResult<Medicine>> SearchMedicinesAsync(
        string? search,
        int page,
        int size,
        CancellationToken ct
    );
    Task<bool> BarcodeExistsAsync(string barcode, CancellationToken ct);
    void RemoveMedicine(Medicine medicine);
    Task AddStockTransactionAsync(StockTransaction transaction, CancellationToken ct);
    Task<int> GetStockBalanceAsync(Guid medicineId, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
