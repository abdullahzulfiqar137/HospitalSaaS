using HospitalSaaS.Domain.Common;

namespace HospitalSaaS.Domain.Insurance;

public sealed class InsurancePolicy : TenantEntity
{
    private InsurancePolicy() { }

    public InsurancePolicy(
        Guid tenantId,
        Guid patientId,
        string provider,
        string policyNumber,
        DateOnly validFrom,
        DateOnly validTo
    )
        : base(tenantId)
    {
        PatientId = patientId;
        Provider = provider;
        PolicyNumber = policyNumber;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    public Guid PatientId { get; private set; }
    public string Provider { get; private set; } = string.Empty;
    public string PolicyNumber { get; private set; } = string.Empty;
    public DateOnly ValidFrom { get; private set; }
    public DateOnly ValidTo { get; private set; }
}
