using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Staff;
using Microsoft.EntityFrameworkCore;

namespace HospitalSaaS.Infrastructure.Persistence;

public sealed class StaffRepository(HospitalDbContext db) : IStaffRepository
{
    public Task AddAsync(StaffMember x, CancellationToken ct) =>
        db.StaffMembers.AddAsync(x, ct).AsTask();

    public Task<bool> EmployeeNumberExistsAsync(string n, CancellationToken ct) =>
        db.StaffMembers.AnyAsync(x => x.EmployeeNumber == n.Trim().ToUpper(), ct);

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);

    public async Task<PagedResult<StaffMember>> SearchAsync(
        string? search,
        int page,
        int size,
        CancellationToken ct
    )
    {
        var q = db.StaffMembers.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            q = q.Where(x =>
                x.EmployeeNumber.Contains(s)
                || x.FirstName.Contains(s)
                || x.LastName.Contains(s)
                || x.JobTitle.Contains(s)
            );
        }
        var c = await q.CountAsync(ct);
        return new(
            await q.OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync(ct),
            page,
            size,
            c
        );
    }
}
