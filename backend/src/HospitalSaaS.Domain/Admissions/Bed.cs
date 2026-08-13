using HospitalSaaS.Domain.Common;
using HospitalSaaS.Domain.Exceptions;

namespace HospitalSaaS.Domain.Admissions;

public sealed class Bed : TenantEntity
{
    private Bed() { }

    public Bed(Guid tenantId, Guid roomId, string number)
        : base(tenantId)
    {
        RoomId = roomId;
        Number = number;
    }

    public Guid RoomId { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public bool IsOccupied { get; private set; }

    public void Occupy()
    {
        if (IsOccupied)
            throw new DomainException("Bed is already occupied.");
        IsOccupied = true;
    }

    public void Release() => IsOccupied = false;
}
