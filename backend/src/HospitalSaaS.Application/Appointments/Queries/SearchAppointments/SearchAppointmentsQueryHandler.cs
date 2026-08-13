using HospitalSaaS.Application.Appointments.Models;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Pagination;

namespace HospitalSaaS.Application.Appointments.Queries.SearchAppointments;

public sealed class SearchAppointmentsQueryHandler(IAppointmentRepository repository)
    : IRequestHandler<SearchAppointmentsQuery, PagedResult<AppointmentDto>>
{
    public async Task<PagedResult<AppointmentDto>> Handle(
        SearchAppointmentsQuery query,
        CancellationToken cancellationToken
    )
    {
        var page = await repository.SearchAsync(
            query.Date,
            query.DoctorId,
            query.PatientId,
            query.Status,
            query.PageNumber,
            query.PageSize,
            cancellationToken
        );
        return new PagedResult<AppointmentDto>(
            page.Items.Select(AppointmentDto.From).ToList(),
            page.PageNumber,
            page.PageSize,
            page.TotalCount
        );
    }
}
