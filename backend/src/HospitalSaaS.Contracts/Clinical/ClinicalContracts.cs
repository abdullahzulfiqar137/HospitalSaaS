namespace HospitalSaaS.Contracts.Clinical;

public sealed record CreateMedicalRecordRequest(
    Guid PatientId,
    Guid DoctorId,
    Guid? AppointmentId,
    string ChiefComplaint,
    string? History,
    string? Examination,
    string? ClinicalNotes,
    string? TreatmentPlan,
    DateOnly? FollowUpDate,
    IReadOnlyList<DiagnosisRequest>? Diagnoses = null
);

public sealed record DiagnosisRequest(string Code, string Description, bool IsPrimary);

public sealed record AddDiagnosisRequest(string Code, string Description, bool IsPrimary);

public sealed record CreatePrescriptionRequest(
    Guid PatientId,
    Guid DoctorId,
    Guid? AppointmentId,
    string? Notes,
    IReadOnlyList<PrescriptionItemRequest> Items
);

public sealed record PrescriptionItemRequest(
    Guid MedicineId,
    string Dose,
    string Frequency,
    string Duration,
    string Route,
    string? Instructions
);

public sealed record CreateRadiologyOrderRequest(
    Guid PatientId,
    Guid DoctorId,
    string Procedure,
    Guid? RadiologyTestId = null
);

public sealed record CreateRadiologyTestRequest(
    string Code,
    string Name,
    string Modality,
    decimal Price
);

public sealed record RecordRadiologyReportRequest(string Report);
