using HospitalSaaS.Domain.Common;

namespace HospitalSaaS.Domain.Admissions;

public sealed class Room : TenantEntity
{
    private Room() { }

    public Room(Guid tenantId, Guid wardId, string number)
        : base(tenantId)
    {
        WardId = wardId;
        Number = number;
    }

    public Guid WardId { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
}
