using HospitalSaaS.Domain.Admissions;

namespace HospitalSaaS.Application.Admissions;

public sealed record WardDto(Guid Id, string Name, string Code, bool IsActive)
{
    public static WardDto From(Ward x) => new(x.Id, x.Name, x.Code, x.IsActive);
}

public sealed record RoomDto(Guid Id, Guid WardId, string Number, bool IsActive)
{
    public static RoomDto From(Room x) => new(x.Id, x.WardId, x.Number, x.IsActive);
}

public sealed record BedDto(Guid Id, Guid RoomId, string Number, bool IsOccupied)
{
    public static BedDto From(Bed x) => new(x.Id, x.RoomId, x.Number, x.IsOccupied);
}

public sealed record AdmissionHistoryDto(
    Guid Id,
    Guid AdmissionId,
    Guid FromWardId,
    Guid FromRoomId,
    Guid FromBedId,
    Guid ToWardId,
    Guid ToRoomId,
    Guid ToBedId,
    string? Reason,
    DateTime TransferredAtUtc
)
{
    public static AdmissionHistoryDto From(AdmissionHistory x) =>
        new(
            x.Id,
            x.AdmissionId,
            x.FromWardId,
            x.FromRoomId,
            x.FromBedId,
            x.ToWardId,
            x.ToRoomId,
            x.ToBedId,
            x.Reason,
            x.TransferredAtUtc
        );
}

public sealed record AdmissionDto(
    Guid Id,
    string AdmissionNumber,
    Guid PatientId,
    Guid DoctorId,
    DateTime AdmissionDateUtc,
    string AdmissionType,
    string Reason,
    Guid WardId,
    Guid RoomId,
    Guid BedId,
    AdmissionStatus Status,
    DateTime? DischargeDateUtc
)
{
    public static AdmissionDto From(Admission x) =>
        new(
            x.Id,
            x.AdmissionNumber,
            x.PatientId,
            x.DoctorId,
            x.AdmissionDateUtc,
            x.AdmissionType,
            x.Reason,
            x.WardId,
            x.RoomId,
            x.BedId,
            x.Status,
            x.DischargeDateUtc
        );
}
