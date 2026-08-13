using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Application.Patients.Models;

namespace HospitalSaaS.Application.Patients.Queries.SearchPatients;

public sealed class SearchPatientsQueryHandler(IPatientRepository repository)
    : IRequestHandler<SearchPatientsQuery, PagedResult<PatientDto>>
{
    public async Task<PagedResult<PatientDto>> Handle(
        SearchPatientsQuery query,
        CancellationToken ct
    )
    {
        var page = Math.Max(1, query.Page);
        var size = Math.Clamp(query.PageSize, 1, 100);
        var result = await repository.SearchAsync(
            query.Search,
            page,
            size,
            query.SortBy,
            query.Descending,
            ct
        );
        return new(result.Items.Select(PatientDto.From).ToList(), page, size, result.TotalCount);
    }
}
