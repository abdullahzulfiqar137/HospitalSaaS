using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Roles;
using HospitalSaaS.Domain.Tenants;
using HospitalSaaS.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace HospitalSaaS.Infrastructure.Persistence;

public sealed class AdministrationRepository(HospitalDbContext db) : IAdministrationRepository
{
    public Task AddTenantAsync(Tenant x, CancellationToken ct) =>
        db.Tenants.AddAsync(x, ct).AsTask();

    public Task<bool> TenantSlugExistsAsync(string s, CancellationToken ct) =>
        db.Tenants.IgnoreQueryFilters().AnyAsync(x => x.Slug == s.Trim().ToLower(), ct);

    public Task AddRoleAsync(Role x, CancellationToken ct) => db.Roles.AddAsync(x, ct).AsTask();

    public Task<bool> RoleNameExistsAsync(string n, CancellationToken ct) =>
        db.Roles.AnyAsync(x => x.Name == n.Trim(), ct);

    public async Task<IReadOnlyList<Permission>> GetPermissionsAsync(
        IReadOnlyCollection<string> n,
        CancellationToken ct
    ) => await db.Permissions.Where(x => n.Contains(x.Name)).ToListAsync(ct);

    public Task AddUserAsync(User x, CancellationToken ct) => db.Users.AddAsync(x, ct).AsTask();

    public Task<bool> UserEmailExistsAsync(string e, CancellationToken ct) =>
        db.Users.AnyAsync(x => x.Email == e, ct);

    public async Task<IReadOnlyList<Role>> GetRolesAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken ct
    ) => await db.Roles.Where(x => ids.Contains(x.Id)).ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);

    public async Task<PagedResult<Role>> SearchRolesAsync(int page, int size, CancellationToken ct)
    {
        var q = db.Roles.AsNoTracking().Include(x => x.Permissions);
        var c = await q.CountAsync(ct);
        return new(
            await q.OrderBy(x => x.Name).Skip((page - 1) * size).Take(size).ToListAsync(ct),
            page,
            size,
            c
        );
    }

    public async Task<PagedResult<User>> SearchUsersAsync(
        string? search,
        int page,
        int size,
        CancellationToken ct
    )
    {
        IQueryable<User> q = db.Users.AsNoTracking().Include(x => x.Roles);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            q = q.Where(x => x.Email.Contains(s));
        }
        var c = await q.CountAsync(ct);
        return new(
            await q.OrderBy(x => x.Email).Skip((page - 1) * size).Take(size).ToListAsync(ct),
            page,
            size,
            c
        );
    }
}
