namespace HospitalSaaS.Contracts.Admissions;

public sealed record WardResponse(Guid Id, string Name, string Code, bool IsActive);

public sealed record RoomResponse(Guid Id, Guid WardId, string Number, bool IsActive);

public sealed record BedResponse(Guid Id, Guid RoomId, string Number, bool IsOccupied);

public sealed record AdmissionResponse(
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
    string Status,
    DateTime? DischargeDateUtc
);

public sealed record AdmissionHistoryResponse(
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
);
