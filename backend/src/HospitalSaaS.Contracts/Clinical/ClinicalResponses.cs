namespace HospitalSaaS.Contracts.Clinical;

public sealed record MedicalRecordResponse(
    Guid Id,
    Guid PatientId,
    Guid DoctorId,
    Guid? AppointmentId,
    string ChiefComplaint,
    string? History,
    string? Examination,
    string? ClinicalNotes,
    string? TreatmentPlan,
    DateOnly? FollowUpDate,
    IReadOnlyList<DiagnosisResponse> Diagnoses
);

public sealed record DiagnosisResponse(Guid Id, string Code, string Description, bool IsPrimary);

public sealed record PrescriptionResponse(
    Guid Id,
    Guid PatientId,
    Guid DoctorId,
    Guid? AppointmentId,
    DateTime DateUtc,
    string? Notes,
    IReadOnlyList<PrescriptionLineResponse> Items
);

public sealed record PrescriptionLineResponse(
    Guid Id,
    Guid MedicineId,
    string Dose,
    string Frequency,
    string Duration,
    string Route,
    string? Instructions
);

public sealed record RadiologyOrderResponse(
    Guid Id,
    Guid PatientId,
    Guid DoctorId,
    Guid? RadiologyTestId,
    string Procedure,
    DateTime OrderedAtUtc,
    string Status,
    string? Report
);

public sealed record RadiologyTestResponse(
    Guid Id,
    string Code,
    string Name,
    string Modality,
    decimal Price,
    bool IsActive
);

public sealed record RadiologyResultResponse(
    Guid Id,
    string Report,
    int Version,
    DateTime RecordedAtUtc
);
