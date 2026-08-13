using HospitalSaaS.Domain.Common;
using HospitalSaaS.Domain.Exceptions;

namespace HospitalSaaS.Domain.Radiology;

public sealed class RadiologyTest : TenantEntity
{
    private RadiologyTest() { }

    public RadiologyTest(Guid tenantId, string code, string name, string modality, decimal price)
        : base(tenantId)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
            throw new DomainException("Radiology test code and name are required.");
        if (price < 0)
            throw new DomainException("Radiology test price cannot be negative.");
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Modality = modality.Trim();
        Price = price;
        IsActive = true;
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Modality { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public bool IsActive { get; private set; }
}
