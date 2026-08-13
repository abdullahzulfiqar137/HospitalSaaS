using HospitalSaaS.Domain.Admissions;
using HospitalSaaS.Domain.Exceptions;

namespace HospitalSaaS.Domain.Tests;

public sealed class AdmissionTests
{
    [Fact]
    public void Discharged_admission_cannot_be_transferred()
    {
        var admission = new Admission(
            Guid.NewGuid(),
            "ADM-1",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Emergency",
            "Reason",
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid()
        );
        admission.Discharge();
        Assert.Throws<DomainException>(() =>
            admission.Transfer(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid())
        );
    }
}
