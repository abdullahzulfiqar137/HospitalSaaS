using HospitalSaaS.Application.Appointments.Models;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;

namespace HospitalSaaS.Application.Appointments.Queries.GetAppointmentById;

public sealed class GetAppointmentByIdQueryHandler(IAppointmentRepository repository)
    : IRequestHandler<GetAppointmentByIdQuery, Result<AppointmentDto>>
{
    public async Task<Result<AppointmentDto>> Handle(
        GetAppointmentByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var appointment = await repository.GetAsync(query.Id, false, cancellationToken);
        return appointment is null
            ? Result<AppointmentDto>.Failure(
                new("Appointments.NotFound", "Appointment was not found.", ErrorType.NotFound)
            )
            : Result<AppointmentDto>.Success(AppointmentDto.From(appointment));
    }
}
