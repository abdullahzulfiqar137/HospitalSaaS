using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Radiology;
using Microsoft.EntityFrameworkCore;

namespace HospitalSaaS.Infrastructure.Persistence;

public sealed class RadiologyRepository(HospitalDbContext dbContext) : IRadiologyRepository
{
    public Task AddTestAsync(RadiologyTest test, CancellationToken ct) =>
        dbContext.RadiologyTests.AddAsync(test, ct).AsTask();

    public Task<RadiologyTest?> GetTestAsync(Guid id, CancellationToken ct) =>
        dbContext.RadiologyTests.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct);

    public Task<bool> TestCodeExistsAsync(string code, CancellationToken ct) =>
        dbContext.RadiologyTests.AnyAsync(x => x.Code == code.Trim().ToUpper(), ct);

    public async Task<PagedResult<RadiologyTest>> SearchTestsAsync(
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken ct
    )
    {
        var query = dbContext.RadiologyTests.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var value = search.Trim();
            query = query.Where(x => x.Code.Contains(value) || x.Name.Contains(value));
        }
        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(x => x.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        return new(items, pageNumber, pageSize, total);
    }

    public Task AddAsync(RadiologyOrder order, CancellationToken ct) =>
        dbContext.RadiologyOrders.AddAsync(order, ct).AsTask();

    public Task AddResultAsync(RadiologyResult result, CancellationToken ct) =>
        dbContext.RadiologyResults.AddAsync(result, ct).AsTask();

    public Task<int> CountResultsAsync(Guid orderId, CancellationToken ct) =>
        dbContext.RadiologyResults.CountAsync(x => x.RadiologyOrderId == orderId, ct);

    public async Task<IReadOnlyList<RadiologyResult>> GetResultsAsync(
        Guid orderId,
        CancellationToken ct
    ) =>
        await dbContext
            .RadiologyResults.AsNoTracking()
            .Where(x => x.RadiologyOrderId == orderId)
            .OrderBy(x => x.Version)
            .ToListAsync(ct);

    public Task<RadiologyOrder?> GetAsync(Guid id, bool tracking, CancellationToken ct)
    {
        var query = tracking ? dbContext.RadiologyOrders : dbContext.RadiologyOrders.AsNoTracking();
        return query.SingleOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<PagedResult<RadiologyOrder>> SearchAsync(
        Guid? patientId,
        int pageNumber,
        int pageSize,
        CancellationToken ct
    )
    {
        var query = dbContext.RadiologyOrders.AsNoTracking();
        if (patientId.HasValue)
            query = query.Where(x => x.PatientId == patientId.Value);
        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(x => x.OrderedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        return new(items, pageNumber, pageSize, totalCount);
    }

    public Task SaveChangesAsync(CancellationToken ct) => dbContext.SaveChangesAsync(ct);
}
