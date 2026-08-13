using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Suppliers;
using Microsoft.EntityFrameworkCore;

namespace HospitalSaaS.Infrastructure.Persistence;

public sealed class SupplierRepository(HospitalDbContext db) : ISupplierRepository
{
    public Task AddAsync(Supplier x, CancellationToken ct) => db.Suppliers.AddAsync(x, ct).AsTask();

    public Task<Supplier?> GetAsync(Guid id, bool tracking, CancellationToken ct) =>
        (tracking ? db.Suppliers : db.Suppliers.AsNoTracking()).SingleOrDefaultAsync(
            x => x.Id == id,
            ct
        );

    public void Remove(Supplier x) => db.Suppliers.Remove(x);

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);

    public async Task<PagedResult<Supplier>> SearchAsync(
        string? search,
        int page,
        int size,
        CancellationToken ct
    )
    {
        var q = db.Suppliers.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            q = q.Where(x =>
                x.Name.Contains(s) || (x.ContactName != null && x.ContactName.Contains(s))
            );
        }
        try
        {
            var c = await q.CountAsync(ct);
            return new(
                await q.OrderBy(x => x.Name).Skip((page - 1) * size).Take(size).ToListAsync(ct),
                page,
                size,
                c
            );
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            return new([], page, size, 0);
        }
    }
}
