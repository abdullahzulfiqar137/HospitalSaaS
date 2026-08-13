using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Application.Doctors.Models;

namespace HospitalSaaS.Application.Doctors.Queries.SearchDoctors;

public sealed class SearchDoctorsQueryHandler(IDoctorRepository repository)
    : IRequestHandler<SearchDoctorsQuery, PagedResult<DoctorDto>>
{
    public async Task<PagedResult<DoctorDto>> Handle(
        SearchDoctorsQuery query,
        CancellationToken cancellationToken
    )
    {
        var page = await repository.SearchAsync(
            query.Search,
            query.PageNumber,
            query.PageSize,
            cancellationToken
        );
        return new PagedResult<DoctorDto>(
            page.Items.Select(DoctorDto.From).ToList(),
            page.PageNumber,
            page.PageSize,
            page.TotalCount
        );
    }
}
