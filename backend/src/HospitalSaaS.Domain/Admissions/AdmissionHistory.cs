using HospitalSaaS.Domain.Common;

namespace HospitalSaaS.Domain.Admissions;

public sealed class AdmissionHistory : TenantEntity
{
    private AdmissionHistory() { }

    public AdmissionHistory(
        Guid tenantId,
        Guid admissionId,
        Guid fromWardId,
        Guid fromRoomId,
        Guid fromBedId,
        Guid toWardId,
        Guid toRoomId,
        Guid toBedId,
        string? reason
    )
        : base(tenantId)
    {
        AdmissionId = admissionId;
        FromWardId = fromWardId;
        FromRoomId = fromRoomId;
        FromBedId = fromBedId;
        ToWardId = toWardId;
        ToRoomId = toRoomId;
        ToBedId = toBedId;
        Reason = reason?.Trim();
        TransferredAtUtc = DateTime.UtcNow;
    }

    public Guid AdmissionId { get; private set; }
    public Guid FromWardId { get; private set; }
    public Guid FromRoomId { get; private set; }
    public Guid FromBedId { get; private set; }
    public Guid ToWardId { get; private set; }
    public Guid ToRoomId { get; private set; }
    public Guid ToBedId { get; private set; }
    public string? Reason { get; private set; }
    public DateTime TransferredAtUtc { get; private set; }
}
