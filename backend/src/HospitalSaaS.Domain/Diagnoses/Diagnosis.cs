using HospitalSaaS.Domain.Common;
using HospitalSaaS.Domain.Exceptions;

namespace HospitalSaaS.Domain.Diagnoses;

public sealed class Diagnosis : TenantEntity
{
    private Diagnosis() { }

    public Diagnosis(
        Guid tenantId,
        Guid medicalRecordId,
        string code,
        string description,
        bool isPrimary
    )
        : base(tenantId)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(description))
            throw new DomainException("Diagnosis code and description are required.");
        MedicalRecordId = medicalRecordId;
        Code = code.Trim().ToUpperInvariant();
        Description = description.Trim();
        IsPrimary = isPrimary;
    }

    public Guid MedicalRecordId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsPrimary { get; private set; }
}
