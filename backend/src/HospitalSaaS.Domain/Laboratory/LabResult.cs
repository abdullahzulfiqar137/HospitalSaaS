using HospitalSaaS.Domain.Common;
using HospitalSaaS.Domain.Exceptions;

namespace HospitalSaaS.Domain.Laboratory;

public sealed class LabResult : TenantEntity
{
    private LabResult() { }

    public LabResult(
        Guid tenantId,
        Guid labOrderId,
        Guid labOrderItemId,
        string value,
        string? unit,
        string? referenceRange,
        int version
    )
        : base(tenantId)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Lab result value is required.");

        LabOrderId = labOrderId;
        LabOrderItemId = labOrderItemId;
        Value = value.Trim();
        Unit = unit?.Trim();
        ReferenceRange = referenceRange?.Trim();
        Version = version;
        RecordedAtUtc = DateTime.UtcNow;
    }

    public Guid LabOrderId { get; private set; }
    public Guid LabOrderItemId { get; private set; }
    public string Value { get; private set; } = string.Empty;
    public string? Unit { get; private set; }
    public string? ReferenceRange { get; private set; }
    public int Version { get; private set; }
    public DateTime RecordedAtUtc { get; private set; }
}
