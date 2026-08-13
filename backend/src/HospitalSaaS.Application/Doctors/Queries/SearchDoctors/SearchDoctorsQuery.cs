using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Application.Doctors.Models;

namespace HospitalSaaS.Application.Doctors.Queries.SearchDoctors;

public sealed record SearchDoctorsQuery(string? Search, int PageNumber = 1, int PageSize = 20)
    : IQuery<PagedResult<DoctorDto>>;
