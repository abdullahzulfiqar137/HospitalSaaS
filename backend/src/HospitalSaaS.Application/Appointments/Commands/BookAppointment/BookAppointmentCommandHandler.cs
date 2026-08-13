using HospitalSaaS.Application.Appointments.Models;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Domain.Appointments;

namespace HospitalSaaS.Application.Appointments.Commands.BookAppointment;

public sealed class BookAppointmentCommandHandler(
    IAppointmentRepository repository,
    ITenantContext tenant
) : IRequestHandler<BookAppointmentCommand, Result<AppointmentDto>>
{
    public async Task<Result<AppointmentDto>> Handle(
        BookAppointmentCommand command,
        CancellationToken cancellationToken
    )
    {
        var endsAtUtc = command.AppointmentDateTimeUtc.AddMinutes(command.DurationMinutes);
        if (
            await repository.HasDoctorConflictAsync(
                command.DoctorId,
                command.AppointmentDateTimeUtc,
                endsAtUtc,
                null,
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

        var appointment = new Appointment(
            tenant.TenantId,
            command.PatientId,
            command.DoctorId,
            command.DepartmentId,
            command.AppointmentDateTimeUtc,
            command.DurationMinutes,
            command.AppointmentType,
            command.Reason,
            command.Notes
        );
        await repository.AddAsync(appointment, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return Result<AppointmentDto>.Success(AppointmentDto.From(appointment));
    }
}
