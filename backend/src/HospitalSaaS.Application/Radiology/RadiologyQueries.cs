using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;

namespace HospitalSaaS.Application.Radiology;

public sealed record GetRadiologyOrderQuery(Guid Id) : IQuery<Result<RadiologyOrderDto>>;

public sealed class GetRadiologyOrderQueryHandler(IRadiologyRepository repository)
    : IRequestHandler<GetRadiologyOrderQuery, Result<RadiologyOrderDto>>
{
    public async Task<Result<RadiologyOrderDto>> Handle(
        GetRadiologyOrderQuery query,
        CancellationToken ct
    )
    {
        var order = await repository.GetAsync(query.Id, false, ct);
        return order is null
            ? Result<RadiologyOrderDto>.Failure(
                new("Radiology.NotFound", "Radiology order was not found.", ErrorType.NotFound)
            )
            : Result<RadiologyOrderDto>.Success(RadiologyOrderDto.From(order));
    }
}

public sealed record SearchRadiologyOrdersQuery(
    Guid? PatientId,
    int PageNumber = 1,
    int PageSize = 20
) : IQuery<PagedResult<RadiologyOrderDto>>;

public sealed class SearchRadiologyOrdersQueryValidator
    : AbstractValidator<SearchRadiologyOrdersQuery>
{
    public SearchRadiologyOrdersQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class SearchRadiologyOrdersQueryHandler(IRadiologyRepository repository)
    : IRequestHandler<SearchRadiologyOrdersQuery, PagedResult<RadiologyOrderDto>>
{
    public async Task<PagedResult<RadiologyOrderDto>> Handle(
        SearchRadiologyOrdersQuery query,
        CancellationToken ct
    )
    {
        var page = await repository.SearchAsync(
            query.PatientId,
            query.PageNumber,
            query.PageSize,
            ct
        );
        return new(
            page.Items.Select(RadiologyOrderDto.From).ToList(),
            page.PageNumber,
            page.PageSize,
            page.TotalCount
        );
    }
}
