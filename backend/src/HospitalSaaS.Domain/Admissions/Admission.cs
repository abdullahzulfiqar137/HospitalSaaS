using HospitalSaaS.Domain.Common;
using HospitalSaaS.Domain.Exceptions;

namespace HospitalSaaS.Domain.Admissions;

public sealed class Admission : AggregateRoot
{
    private Admission() { }

    public Admission(
        Guid tenantId,
        string number,
        Guid patientId,
        Guid doctorId,
        string type,
        string reason,
        Guid wardId,
        Guid roomId,
        Guid bedId
    )
        : base(tenantId)
    {
        AdmissionNumber = number;
        PatientId = patientId;
        DoctorId = doctorId;
        AdmissionType = type;
        Reason = reason;
        WardId = wardId;
        RoomId = roomId;
        BedId = bedId;
        AdmissionDateUtc = DateTime.UtcNow;
        Status = AdmissionStatus.Admitted;
    }

    public string AdmissionNumber { get; private set; } = string.Empty;
    public Guid PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    public DateTime AdmissionDateUtc { get; private set; }
    public string AdmissionType { get; private set; } = string.Empty;
    public string Reason { get; private set; } = string.Empty;
    public Guid WardId { get; private set; }
    public Guid RoomId { get; private set; }
    public Guid BedId { get; private set; }
    public AdmissionStatus Status { get; private set; }
    public DateTime? DischargeDateUtc { get; private set; }

    public void Transfer(Guid wardId, Guid roomId, Guid bedId)
    {
        if (Status != AdmissionStatus.Admitted)
            throw new DomainException("Only active admissions can be transferred.");
        WardId = wardId;
        RoomId = roomId;
        BedId = bedId;
    }

    public void Discharge()
    {
        if (Status != AdmissionStatus.Admitted)
            throw new DomainException("Admission is not active.");
        Status = AdmissionStatus.Discharged;
        DischargeDateUtc = DateTime.UtcNow;
    }
}

public enum AdmissionStatus
{
    Admitted,
    Discharged,
    Cancelled,
}
