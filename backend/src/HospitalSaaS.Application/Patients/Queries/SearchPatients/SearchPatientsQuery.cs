using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Application.Patients.Models;

namespace HospitalSaaS.Application.Patients.Queries.SearchPatients;

public sealed record SearchPatientsQuery(
    string? Search,
    int Page,
    int PageSize,
    string SortBy,
    bool Descending
) : IQuery<PagedResult<PatientDto>>;
