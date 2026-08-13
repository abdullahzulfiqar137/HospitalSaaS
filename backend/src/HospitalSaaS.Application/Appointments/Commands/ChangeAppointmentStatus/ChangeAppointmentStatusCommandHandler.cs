using HospitalSaaS.Application.Appointments.Models;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;

namespace HospitalSaaS.Application.Appointments.Commands.ChangeAppointmentStatus;

public sealed class ChangeAppointmentStatusCommandHandler(IAppointmentRepository repository)
    : IRequestHandler<ChangeAppointmentStatusCommand, Result<AppointmentDto>>
{
    public async Task<Result<AppointmentDto>> Handle(
        ChangeAppointmentStatusCommand command,
        CancellationToken cancellationToken
    )
    {
        var appointment = await repository.GetAsync(command.Id, true, cancellationToken);
        if (appointment is null)
            return Result<AppointmentDto>.Failure(
                new("Appointments.NotFound", "Appointment was not found.", ErrorType.NotFound)
            );

        if (command.Action == AppointmentAction.Reschedule)
        {
            var startsAtUtc = command.AppointmentDateTimeUtc!.Value;
            var duration = command.DurationMinutes ?? appointment.DurationMinutes;
            if (
                await repository.HasDoctorConflictAsync(
                    appointment.DoctorId,
                    startsAtUtc,
                    startsAtUtc.AddMinutes(duration),
                    appointment.Id,
                    cancellationToken
                )
            )
                return Result<AppointmentDto>.Failure(
                    new(
                        "Appointments.Conflict",
                        "Doctor already has an overlapping appointment.",
                        ErrorType.Conflict
                    )
                );
            appointment.Reschedule(startsAtUtc, duration);
        }
        else
        {
            ApplyTransition(appointment, command);
        }

        await repository.SaveChangesAsync(cancellationToken);
        return Result<AppointmentDto>.Success(AppointmentDto.From(appointment));
    }

    private static void ApplyTransition(
        HospitalSaaS.Domain.Appointments.Appointment appointment,
        ChangeAppointmentStatusCommand command
    )
    {
        switch (command.Action)
        {
            case AppointmentAction.Confirm:
                appointment.Confirm();
                break;
            case AppointmentAction.CheckIn:
                appointment.CheckIn();
                break;
            case AppointmentAction.Start:
                appointment.Start();
                break;
            case AppointmentAction.Complete:
                appointment.Complete();
                break;
            case AppointmentAction.NoShow:
                appointment.MarkNoShow();
                break;
            case AppointmentAction.Cancel:
                appointment.Cancel(command.Reason!);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(command.Action));
        }
    }
}
