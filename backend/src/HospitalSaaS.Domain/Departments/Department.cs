using HospitalSaaS.Domain.Common;

namespace HospitalSaaS.Domain.Departments;

public sealed class Department : TenantEntity
{
    private Department() { }

    public Department(Guid tenantId, string name, string code, string? description)
        : base(tenantId)
    {
        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        Description = description;
        Status = DepartmentStatus.Active;
    }

    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid? HeadDoctorId { get; private set; }
    public DepartmentStatus Status { get; private set; }

    public void Update(string name, string code, string? description, Guid? headDoctorId)
    {
        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        Description = description;
        HeadDoctorId = headDoctorId;
    }
}

public enum DepartmentStatus
{
    Active,
    Inactive,
}
