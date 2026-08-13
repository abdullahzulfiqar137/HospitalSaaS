using HospitalSaaS.Domain.Diagnoses;
using HospitalSaaS.Domain.MedicalRecords;
using HospitalSaaS.Domain.Prescriptions;

namespace HospitalSaaS.Application.Clinical;

public sealed record MedicalRecordDto(
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
    IReadOnlyList<DiagnosisDto> Diagnoses
)
{
    public static MedicalRecordDto From(MedicalRecord x) =>
        new(
            x.Id,
            x.PatientId,
            x.DoctorId,
            x.AppointmentId,
            x.ChiefComplaint,
            x.History,
            x.Examination,
            x.ClinicalNotes,
            x.TreatmentPlan,
            x.FollowUpDate,
            x.Diagnoses.Select(DiagnosisDto.From).ToList()
        );
}

public sealed record DiagnosisDto(Guid Id, string Code, string Description, bool IsPrimary)
{
    public static DiagnosisDto From(Diagnosis diagnosis) =>
        new(diagnosis.Id, diagnosis.Code, diagnosis.Description, diagnosis.IsPrimary);
}

public sealed record PrescriptionLineDto(
    Guid Id,
    Guid MedicineId,
    string Dose,
    string Frequency,
    string Duration,
    string Route,
    string? Instructions
);

public sealed record PrescriptionDto(
    Guid Id,
    Guid PatientId,
    Guid DoctorId,
    Guid? AppointmentId,
    DateTime DateUtc,
    string? Notes,
    IReadOnlyList<PrescriptionLineDto> Items
)
{
    public static PrescriptionDto From(Prescription x) =>
        new(
            x.Id,
            x.PatientId,
            x.DoctorId,
            x.AppointmentId,
            x.DateUtc,
            x.Notes,
            x.Items.Select(i => new PrescriptionLineDto(
                    i.Id,
                    i.MedicineId,
                    i.Dose,
                    i.Frequency,
                    i.Duration,
                    i.Route,
                    i.Instructions
                ))
                .ToList()
        );
}
