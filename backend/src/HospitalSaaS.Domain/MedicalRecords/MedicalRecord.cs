using HospitalSaaS.Domain.Common;
using HospitalSaaS.Domain.Diagnoses;
using HospitalSaaS.Domain.Exceptions;

namespace HospitalSaaS.Domain.MedicalRecords;

public sealed class MedicalRecord : TenantEntity
{
    private readonly List<Diagnosis> _diagnoses = [];

    private MedicalRecord() { }

    public MedicalRecord(
        Guid tenantId,
        Guid patientId,
        Guid doctorId,
        Guid? appointmentId,
        string complaint,
        string? history,
        string? examination,
        string? clinicalNotes,
        string? treatmentPlan,
        DateOnly? followUpDate
    )
        : base(tenantId)
    {
        PatientId = patientId;
        DoctorId = doctorId;
        AppointmentId = appointmentId;
        ChiefComplaint = complaint;
        History = history;
        Examination = examination;
        ClinicalNotes = clinicalNotes;
        TreatmentPlan = treatmentPlan;
        FollowUpDate = followUpDate;
    }

    public Guid PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    public Guid? AppointmentId { get; private set; }
    public string ChiefComplaint { get; private set; } = string.Empty;
    public string? History { get; private set; }
    public string? Examination { get; private set; }
    public string? ClinicalNotes { get; private set; }
    public string? TreatmentPlan { get; private set; }
    public DateOnly? FollowUpDate { get; private set; }
    public IReadOnlyCollection<Diagnosis> Diagnoses => _diagnoses;

    public Diagnosis AddDiagnosis(string code, string description, bool isPrimary)
    {
        if (isPrimary && _diagnoses.Any(x => x.IsPrimary))
            throw new DomainException("A medical record can have only one primary diagnosis.");

        var diagnosis = new Diagnosis(TenantId, Id, code, description, isPrimary);
        _diagnoses.Add(diagnosis);
        return diagnosis;
    }
}
