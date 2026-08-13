using HospitalSaaS.Domain.Exceptions;
using HospitalSaaS.Domain.Patients;

namespace HospitalSaaS.Domain.Tests;

public sealed class PatientTests
{
    [Fact]
    public void Future_date_of_birth_is_rejected()
    {
        var future = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        Assert.Throws<DomainException>(() => Create(future));
    }

    [Fact]
    public void New_patient_is_active_and_tenant_owned()
    {
        var tenant = Guid.NewGuid();
        var patient = Create(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30)), tenant);
        Assert.Equal(tenant, patient.TenantId);
        Assert.Equal(PatientStatus.Active, patient.Status);
    }

    private static Patient Create(DateOnly dob, Guid? tenant = null) =>
        new(
            tenant ?? Guid.NewGuid(),
            "PAT-2026-000001",
            "Ali",
            null,
            "Khan",
            dob,
            "Male",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null
        );
}
