using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Pagination;

namespace HospitalSaaS.Application.Departments;

public sealed record SearchDepartmentsQuery(string? Search, int PageNumber = 1, int PageSize = 20)
    : IQuery<PagedResult<DepartmentDto>>;

public sealed class SearchDepartmentsQueryValidator : AbstractValidator<SearchDepartmentsQuery>
{
    public SearchDepartmentsQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class SearchDepartmentsQueryHandler(IDepartmentRepository repository)
    : IRequestHandler<SearchDepartmentsQuery, PagedResult<DepartmentDto>>
{
    public async Task<PagedResult<DepartmentDto>> Handle(
        SearchDepartmentsQuery query,
        CancellationToken cancellationToken
    )
    {
        var page = await repository.SearchAsync(
            query.Search,
            query.PageNumber,
            query.PageSize,
            cancellationToken
        );
        return new(
            page.Items.Select(DepartmentDto.From).ToList(),
            page.PageNumber,
            page.PageSize,
            page.TotalCount
        );
    }
}
