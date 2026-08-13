using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Roles;
using HospitalSaaS.Domain.Tenants;
using HospitalSaaS.Domain.Users;

namespace HospitalSaaS.Application.Common.Interfaces;

public interface IAdministrationRepository
{
    Task AddTenantAsync(Tenant x, CancellationToken ct);
    Task<bool> TenantSlugExistsAsync(string slug, CancellationToken ct);
    Task AddRoleAsync(Role x, CancellationToken ct);
    Task<bool> RoleNameExistsAsync(string name, CancellationToken ct);
    Task<IReadOnlyList<Permission>> GetPermissionsAsync(
        IReadOnlyCollection<string> names,
        CancellationToken ct
    );
    Task<PagedResult<Role>> SearchRolesAsync(int page, int size, CancellationToken ct);
    Task AddUserAsync(User x, CancellationToken ct);
    Task<bool> UserEmailExistsAsync(string email, CancellationToken ct);
    Task<IReadOnlyList<Role>> GetRolesAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct);
    Task<PagedResult<User>> SearchUsersAsync(
        string? search,
        int page,
        int size,
        CancellationToken ct
    );
    Task SaveChangesAsync(CancellationToken ct);
}
