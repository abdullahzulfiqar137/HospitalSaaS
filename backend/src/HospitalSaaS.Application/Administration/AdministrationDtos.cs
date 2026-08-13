using HospitalSaaS.Domain.Roles;
using HospitalSaaS.Domain.Tenants;
using HospitalSaaS.Domain.Users;

namespace HospitalSaaS.Application.Administration;

public sealed record TenantDto(Guid Id, string Name, string Slug, bool IsActive)
{
    public static TenantDto From(Tenant x) => new(x.Id, x.Name, x.Slug, x.IsActive);
}

public sealed record RoleDto(Guid Id, string Name, IReadOnlyList<string> Permissions)
{
    public static RoleDto From(Role x) =>
        new(x.Id, x.Name, x.Permissions.Select(p => p.Name).ToList());
}

public sealed record UserDto(Guid Id, string Email, bool IsActive, IReadOnlyList<string> Roles)
{
    public static UserDto From(User x) =>
        new(x.Id, x.Email, x.IsActive, x.Roles.Select(r => r.Name).ToList());
}
