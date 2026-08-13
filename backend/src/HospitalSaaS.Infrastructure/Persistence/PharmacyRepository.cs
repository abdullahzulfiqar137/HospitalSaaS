using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Pharmacy;
using Microsoft.EntityFrameworkCore;

namespace HospitalSaaS.Infrastructure.Persistence;

public sealed class PharmacyRepository(HospitalDbContext db) : IPharmacyRepository
{
    public Task AddMedicineAsync(Medicine x, CancellationToken ct) =>
        db.Medicines.AddAsync(x, ct).AsTask();

    public Task<Medicine?> GetMedicineAsync(Guid id, bool tracking, CancellationToken ct) =>
        (tracking ? db.Medicines : db.Medicines.AsNoTracking()).SingleOrDefaultAsync(
            x => x.Id == id,
            ct
        );

    public Task<bool> BarcodeExistsAsync(string barcode, CancellationToken ct) =>
        db.Medicines.AnyAsync(x => x.Barcode == barcode.Trim(), ct);

    public void RemoveMedicine(Medicine x) => db.Medicines.Remove(x);

    public Task AddStockTransactionAsync(StockTransaction x, CancellationToken ct) =>
        db.StockTransactions.AddAsync(x, ct).AsTask();

    public Task<int> GetStockBalanceAsync(Guid id, CancellationToken ct) =>
        db
            .StockTransactions.Where(x => x.MedicineId == id)
            .SumAsync(x => (int?)x.Quantity, ct)
            .ContinueWith(x => x.Result ?? 0, ct);

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);

    public async Task<PagedResult<Medicine>> SearchMedicinesAsync(
        string? search,
        int page,
        int size,
        CancellationToken ct
    )
    {
        var q = db.Medicines.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            q = q.Where(x =>
                x.Name.Contains(s)
                || x.GenericName.Contains(s)
                || (x.BrandName != null && x.BrandName.Contains(s))
                || (x.Barcode != null && x.Barcode.Contains(s))
            );
        }
        var count = await q.CountAsync(ct);
        var items = await q.OrderBy(x => x.Name).Skip((page - 1) * size).Take(size).ToListAsync(ct);
        return new(items, page, size, count);
    }
}
