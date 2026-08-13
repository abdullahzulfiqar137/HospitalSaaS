namespace HospitalSaaS.Contracts.Appointments;

public sealed record BookAppointmentRequest(
    Guid PatientId,
    Guid DoctorId,
    Guid DepartmentId,
    DateTime AppointmentDateTimeUtc,
    int DurationMinutes,
    string AppointmentType,
    string Reason,
    string? Notes
);

public sealed record AppointmentActionRequest(
    string Action,
    string? Reason,
    DateTime? AppointmentDateTimeUtc,
    int? DurationMinutes
);

public sealed record UpdateAppointmentRequest(
    Guid DoctorId,
    Guid DepartmentId,
    DateTime AppointmentDateTimeUtc,
    int DurationMinutes,
    string AppointmentType,
    string Reason,
    string? Notes
);

public sealed record AppointmentResponse(
    Guid Id,
    Guid PatientId,
    Guid DoctorId,
    Guid DepartmentId,
    DateTime AppointmentDateTimeUtc,
    int DurationMinutes,
    string AppointmentType,
    string Status,
    string Reason,
    string? Notes
);
