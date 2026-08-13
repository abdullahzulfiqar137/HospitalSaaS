using HospitalSaaS.Domain.Common;

namespace HospitalSaaS.Domain.Admissions;

public sealed class Ward : TenantEntity
{
    private Ward() { }

    public Ward(Guid tenantId, string name, string code)
        : base(tenantId)
    {
        Name = name;
        Code = code;
    }

    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
}
