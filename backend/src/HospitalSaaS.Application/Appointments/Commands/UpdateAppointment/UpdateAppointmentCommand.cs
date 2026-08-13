using HospitalSaaS.Application.Appointments.Models;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;

namespace HospitalSaaS.Application.Appointments.Commands.UpdateAppointment;

public sealed record UpdateAppointmentCommand(
    Guid Id,
    Guid DoctorId,
    Guid DepartmentId,
    DateTime AppointmentDateTimeUtc,
    int DurationMinutes,
    string AppointmentType,
    string Reason,
    string? Notes
) : ICommand<Result<AppointmentDto>>;
