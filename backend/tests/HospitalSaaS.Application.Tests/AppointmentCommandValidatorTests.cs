using HospitalSaaS.Application.Appointments.Commands.BookAppointment;
using HospitalSaaS.Application.Appointments.Commands.ChangeAppointmentStatus;

namespace HospitalSaaS.Application.Tests;

public sealed class AppointmentCommandValidatorTests
{
    [Fact]
    public async Task Booking_Requires_Utc_Time_Valid_Duration_And_Identifiers()
    {
        var command = new BookAppointmentCommand(
            Guid.Empty,
            Guid.Empty,
            Guid.Empty,
            DateTime.Now,
            1,
            "",
            "",
            null
        );

        var result = await new BookAppointmentCommandValidator().ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            x => x.PropertyName == nameof(command.AppointmentDateTimeUtc)
        );
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.DurationMinutes));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.PatientId));
    }

    [Fact]
    public async Task Cancellation_Requires_A_Reason()
    {
        var command = new ChangeAppointmentStatusCommand(
            Guid.NewGuid(),
            AppointmentAction.Cancel,
            null,
            null,
            null
        );

        var result = await new ChangeAppointmentStatusCommandValidator().ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.Reason));
    }

    [Fact]
    public async Task Reschedule_Requires_A_Utc_Date()
    {
        var command = new ChangeAppointmentStatusCommand(
            Guid.NewGuid(),
            AppointmentAction.Reschedule,
            null,
            DateTime.Now,
            30
        );

        var result = await new ChangeAppointmentStatusCommandValidator().ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            x => x.PropertyName.Contains(nameof(command.AppointmentDateTimeUtc))
        );
    }
}
