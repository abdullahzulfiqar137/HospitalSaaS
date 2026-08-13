using HospitalSaaS.Domain.Common;
using HospitalSaaS.Domain.Exceptions;

namespace HospitalSaaS.Domain.Radiology;

public sealed class RadiologyResult : TenantEntity
{
    private RadiologyResult() { }

    public RadiologyResult(Guid tenantId, Guid orderId, string report, int version)
        : base(tenantId)
    {
        if (string.IsNullOrWhiteSpace(report))
            throw new DomainException("Radiology report is required.");
        RadiologyOrderId = orderId;
        Report = report.Trim();
        Version = version;
        RecordedAtUtc = DateTime.UtcNow;
    }

    public Guid RadiologyOrderId { get; private set; }
    public string Report { get; private set; } = string.Empty;
    public int Version { get; private set; }
    public DateTime RecordedAtUtc { get; private set; }
}
