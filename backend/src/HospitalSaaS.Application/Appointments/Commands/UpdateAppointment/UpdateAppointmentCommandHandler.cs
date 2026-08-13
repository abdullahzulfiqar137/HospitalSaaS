using HospitalSaaS.Application.Appointments.Models;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;

namespace HospitalSaaS.Application.Appointments.Commands.UpdateAppointment;

public sealed class UpdateAppointmentCommandHandler(IAppointmentRepository repository)
    : IRequestHandler<UpdateAppointmentCommand, Result<AppointmentDto>>
{
    public async Task<Result<AppointmentDto>> Handle(
        UpdateAppointmentCommand command,
        CancellationToken cancellationToken
    )
    {
        var appointment = await repository.GetAsync(command.Id, true, cancellationToken);
        if (appointment is null)
            return Result<AppointmentDto>.Failure(
                new("Appointments.NotFound", "Appointment was not found.", ErrorType.NotFound)
            );

        if (
            await repository.HasDoctorConflictAsync(
                command.DoctorId,
                command.AppointmentDateTimeUtc,
                command.AppointmentDateTimeUtc.AddMinutes(command.DurationMinutes),
                command.Id,
                cancellationToken
            )
        )
            return Result<AppointmentDto>.Failure(
                new(
                    "Appointments.Conflict",
                    "The doctor already has an appointment in this time range.",
                    ErrorType.Conflict
                )
            );

        appointment.Update(
            command.DoctorId,
            command.DepartmentId,
            command.AppointmentDateTimeUtc,
            command.DurationMinutes,
            command.AppointmentType,
            command.Reason,
            command.Notes
        );
        await repository.SaveChangesAsync(cancellationToken);
        return Result<AppointmentDto>.Success(AppointmentDto.From(appointment));
    }
}
