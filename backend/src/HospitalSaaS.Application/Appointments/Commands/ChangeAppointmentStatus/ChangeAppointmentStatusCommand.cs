using HospitalSaaS.Application.Appointments.Models;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;

namespace HospitalSaaS.Application.Appointments.Commands.ChangeAppointmentStatus;

public enum AppointmentAction
{
    Confirm,
    CheckIn,
    Start,
    Complete,
    NoShow,
    Cancel,
    Reschedule,
}

public sealed record ChangeAppointmentStatusCommand(
    Guid Id,
    AppointmentAction Action,
    string? Reason,
    DateTime? AppointmentDateTimeUtc,
    int? DurationMinutes
) : ICommand<Result<AppointmentDto>>;
