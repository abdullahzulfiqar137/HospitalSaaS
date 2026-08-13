using HospitalSaaS.Application.Appointments.Models;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Appointments;

namespace HospitalSaaS.Application.Appointments.Queries.SearchAppointments;

public sealed record SearchAppointmentsQuery(
    DateTime? Date,
    Guid? DoctorId,
    Guid? PatientId,
    AppointmentStatus? Status,
    int PageNumber = 1,
    int PageSize = 20
) : IQuery<PagedResult<AppointmentDto>>;
