using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Departments;
using Microsoft.EntityFrameworkCore;

namespace HospitalSaaS.Infrastructure.Persistence;

public sealed class DepartmentRepository(HospitalDbContext dbContext) : IDepartmentRepository
{
    public Task AddAsync(Department department, CancellationToken cancellationToken) =>
        dbContext.Departments.AddAsync(department, cancellationToken).AsTask();

    public Task<Department?> GetAsync(Guid id, bool tracking, CancellationToken cancellationToken)
    {
        var query = tracking ? dbContext.Departments : dbContext.Departments.AsNoTracking();
        return query.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> CodeExistsAsync(
        string code,
        Guid? excludingId,
        CancellationToken cancellationToken
    ) =>
        dbContext.Departments.AnyAsync(
            x =>
                x.Code == code.Trim().ToUpperInvariant()
                && (!excludingId.HasValue || x.Id != excludingId),
            cancellationToken
        );

    public async Task<PagedResult<Department>> SearchAsync(
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = dbContext.Departments.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x => x.Name.Contains(term) || x.Code.Contains(term));
        }
        var count = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return new(items, pageNumber, pageSize, count);
    }

    public void Remove(Department department) => dbContext.Departments.Remove(department);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
