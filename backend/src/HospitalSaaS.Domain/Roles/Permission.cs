using HospitalSaaS.Domain.Common;

namespace HospitalSaaS.Domain.Roles;

public sealed class Permission : BaseEntity
{
    private Permission() { }

    public Permission(string name) => Name = name;

    public string Name { get; private set; } = string.Empty;
}
