using HospitalSaaS.Domain.Common;

namespace HospitalSaaS.Domain.Radiology;

public sealed class RadiologyOrder : TenantEntity
{
    private RadiologyOrder() { }

    public RadiologyOrder(
        Guid tenantId,
        Guid patientId,
        Guid doctorId,
        string procedure,
        Guid? radiologyTestId = null
    )
        : base(tenantId)
    {
        PatientId = patientId;
        DoctorId = doctorId;
        RadiologyTestId = radiologyTestId;
        Procedure = procedure;
        OrderedAtUtc = DateTime.UtcNow;
        Status = RadiologyOrderStatus.Ordered;
    }

    public Guid PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    public Guid? RadiologyTestId { get; private set; }
    public string Procedure { get; private set; } = string.Empty;
    public DateTime OrderedAtUtc { get; private set; }
    public RadiologyOrderStatus Status { get; private set; }
    public string? Report { get; private set; }

    public void RecordReport(string report)
    {
        Report = report;
        Status = RadiologyOrderStatus.Completed;
    }
}

public enum RadiologyOrderStatus
{
    Ordered,
    Scheduled,
    InProgress,
    Completed,
    Cancelled,
}
