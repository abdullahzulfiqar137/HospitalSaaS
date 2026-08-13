using HospitalSaaS.Domain.Common;
using HospitalSaaS.Domain.Exceptions;

namespace HospitalSaaS.Domain.Laboratory;

public sealed class LabTest : TenantEntity
{
    private LabTest() { }

    public LabTest(
        Guid tenantId,
        string name,
        string code,
        string category,
        decimal price,
        string? normalRange,
        string? unit
    )
        : base(tenantId)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(code))
            throw new DomainException("Lab test name and code are required.");
        if (price < 0)
            throw new DomainException("Lab test price cannot be negative.");

        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        Category = category.Trim();
        Price = price;
        NormalRange = normalRange?.Trim();
        Unit = unit?.Trim();
        IsActive = true;
    }

    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public string? NormalRange { get; private set; }
    public string? Unit { get; private set; }
    public bool IsActive { get; private set; }
}
