using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Laboratory;

namespace HospitalSaaS.Application.Laboratory;

public sealed record GetLabOrderQuery(Guid Id) : IQuery<Result<LabOrderDto>>;

public sealed class GetLabOrderQueryHandler(ILaboratoryRepository repo)
    : IRequestHandler<GetLabOrderQuery, Result<LabOrderDto>>
{
    public async Task<Result<LabOrderDto>> Handle(GetLabOrderQuery x, CancellationToken ct)
    {
        var e = await repo.GetAsync(x.Id, false, ct);
        return e is null
            ? Result<LabOrderDto>.Failure(
                new("Lab.NotFound", "Lab order was not found.", ErrorType.NotFound)
            )
            : Result<LabOrderDto>.Success(LabOrderDto.From(e));
    }
}

public sealed record SearchLabOrdersQuery(
    Guid? PatientId,
    LabOrderStatus? Status,
    int PageNumber,
    int PageSize
) : IQuery<PagedResult<LabOrderDto>>;

public sealed class SearchLabOrdersQueryValidator : AbstractValidator<SearchLabOrdersQuery>
{
    public SearchLabOrdersQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class SearchLabOrdersQueryHandler(ILaboratoryRepository repo)
    : IRequestHandler<SearchLabOrdersQuery, PagedResult<LabOrderDto>>
{
    public async Task<PagedResult<LabOrderDto>> Handle(SearchLabOrdersQuery x, CancellationToken ct)
    {
        var p = await repo.SearchAsync(x.PatientId, x.Status, x.PageNumber, x.PageSize, ct);
        return new(
            p.Items.Select(LabOrderDto.From).ToList(),
            p.PageNumber,
            p.PageSize,
            p.TotalCount
        );
    }
}
