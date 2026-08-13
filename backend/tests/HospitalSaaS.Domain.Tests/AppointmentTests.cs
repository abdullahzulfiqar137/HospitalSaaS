using HospitalSaaS.Domain.Appointments;
using HospitalSaaS.Domain.Exceptions;

namespace HospitalSaaS.Domain.Tests;

public sealed class AppointmentTests
{
    [Fact]
    public void Completed_appointment_cannot_be_cancelled()
    {
        var x = Create();
        x.CheckIn();
        x.Start();
        x.Complete();
        Assert.Throws<DomainException>(() => x.Cancel("changed"));
    }

    [Fact]
    public void Invalid_transition_is_rejected()
    {
        var x = Create();
        Assert.Throws<DomainException>(x.Complete);
    }

    private static Appointment Create() =>
        new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.SpecifyKind(DateTime.UtcNow.AddDays(1), DateTimeKind.Utc),
            30,
            "Consultation",
            "Checkup",
            null
        );
}
