namespace HospitalSaaS.Contracts.Admissions;

public sealed record CreateWardRequest(string Name, string Code);

public sealed record CreateRoomRequest(Guid WardId, string Number);

public sealed record CreateBedRequest(Guid RoomId, string Number);

public sealed record AdmitPatientRequest(
    Guid PatientId,
    Guid DoctorId,
    string AdmissionType,
    string Reason,
    Guid WardId,
    Guid RoomId,
    Guid BedId
);

public sealed record TransferAdmissionRequest(
    Guid WardId,
    Guid RoomId,
    Guid BedId,
    string? Reason = null
);
