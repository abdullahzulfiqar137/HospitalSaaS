using HospitalSaaS.Domain.Common;

namespace HospitalSaaS.Domain.Suppliers;

public sealed class Supplier : TenantEntity
{
    private Supplier() { }

    public Supplier(
        Guid tenantId,
        string name,
        string? contactName,
        string? phone,
        string? email,
        string? address
    )
        : base(tenantId)
    {
        Name = name;
        ContactName = contactName;
        Phone = phone;
        Email = email;
        Address = address;
        IsActive = true;
    }

    public string Name { get; private set; } = string.Empty;
    public string? ContactName { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public string? Address { get; private set; }
    public bool IsActive { get; private set; }
}
