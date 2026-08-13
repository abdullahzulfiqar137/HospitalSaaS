using HospitalSaaS.Domain.Common;

namespace HospitalSaaS.Domain.Roles;

public sealed class Role : TenantEntity
{
    private readonly List<Permission> _permissions = [];

    private Role() { }

    public Role(Guid tenantId, string name)
        : base(tenantId) => Name = name.Trim();

    public string Name { get; private set; } = string.Empty;
    public IReadOnlyCollection<Permission> Permissions => _permissions;

    public void Grant(Permission permission)
    {
        if (_permissions.All(x => x.Id != permission.Id))
            _permissions.Add(permission);
    }
}
