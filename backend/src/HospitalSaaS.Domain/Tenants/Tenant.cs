using HospitalSaaS.Domain.Common;

namespace HospitalSaaS.Domain.Tenants;

public sealed class Tenant : AuditableEntity
{
    private Tenant() { }

    public Tenant(string name, string slug)
    {
        Name = name.Trim();
        Slug = slug.Trim().ToLowerInvariant();
        IsActive = true;
    }

    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
}
