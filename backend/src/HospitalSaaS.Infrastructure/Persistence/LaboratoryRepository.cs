using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Laboratory;
using Microsoft.EntityFrameworkCore;

namespace HospitalSaaS.Infrastructure.Persistence;

public sealed class LaboratoryRepository(HospitalDbContext db) : ILaboratoryRepository
{
    public Task AddTestAsync(LabTest x, CancellationToken ct) =>
        db.LabTests.AddAsync(x, ct).AsTask();

    public Task<LabTest?> GetTestAsync(Guid id, CancellationToken ct) =>
        db.LabTests.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<LabTest>> GetTestsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken ct
    ) => await db.LabTests.AsNoTracking().Where(x => ids.Contains(x.Id)).ToListAsync(ct);

    public Task<bool> TestCodeExistsAsync(string code, CancellationToken ct) =>
        db.LabTests.AnyAsync(x => x.Code == code.Trim().ToUpper(), ct);

    public async Task<PagedResult<LabTest>> SearchTestsAsync(
        string? search,
        int page,
        int size,
        CancellationToken ct
    )
    {
        var query = db.LabTests.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var value = search.Trim();
            query = query.Where(x => x.Code.Contains(value) || x.Name.Contains(value));
        }
        var count = await query.CountAsync(ct);
        var items = await query
            .OrderBy(x => x.Name)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(ct);
        return new(items, page, size, count);
    }

    public Task AddAsync(LabOrder x, CancellationToken ct) => db.LabOrders.AddAsync(x, ct).AsTask();

    public Task AddResultAsync(LabResult x, CancellationToken ct) =>
        db.LabResults.AddAsync(x, ct).AsTask();

    public Task<int> CountResultsAsync(Guid itemId, CancellationToken ct) =>
        db.LabResults.CountAsync(x => x.LabOrderItemId == itemId, ct);

    public async Task<IReadOnlyList<LabResult>> GetResultsAsync(
        Guid orderId,
        CancellationToken ct
    ) =>
        await db
            .LabResults.AsNoTracking()
            .Where(x => x.LabOrderId == orderId)
            .OrderBy(x => x.LabOrderItemId)
            .ThenBy(x => x.Version)
            .ToListAsync(ct);

    public Task<LabOrder?> GetAsync(Guid id, bool tracking, CancellationToken ct) =>
        (tracking ? db.LabOrders : db.LabOrders.AsNoTracking())
            .Include(x => x.Items)
            .SingleOrDefaultAsync(x => x.Id == id, ct);

    public async Task<PagedResult<LabOrder>> SearchAsync(
        Guid? patientId,
        LabOrderStatus? status,
        int page,
        int size,
        CancellationToken ct
    )
    {
        IQueryable<LabOrder> q = db.LabOrders.AsNoTracking().Include(x => x.Items);
        if (patientId.HasValue)
            q = q.Where(x => x.PatientId == patientId);
        if (status.HasValue)
            q = q.Where(x => x.Status == status);
        var count = await q.CountAsync(ct);
        var items = await q.OrderByDescending(x => x.OrderedAtUtc)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(ct);
        return new(items, page, size, count);
    }

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
