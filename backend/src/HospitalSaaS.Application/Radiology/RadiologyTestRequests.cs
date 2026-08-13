using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Radiology;

namespace HospitalSaaS.Application.Radiology;

public sealed record CreateRadiologyTestCommand(
    string Code,
    string Name,
    string Modality,
    decimal Price
) : ICommand<Result<RadiologyTestDto>>;

public sealed class CreateRadiologyTestCommandValidator
    : AbstractValidator<CreateRadiologyTestCommand>
{
    public CreateRadiologyTestCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Modality).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
    }
}

public sealed class CreateRadiologyTestCommandHandler(
    IRadiologyRepository repository,
    ITenantContext tenant
) : IRequestHandler<CreateRadiologyTestCommand, Result<RadiologyTestDto>>
{
    public async Task<Result<RadiologyTestDto>> Handle(
        CreateRadiologyTestCommand command,
        CancellationToken cancellationToken
    )
    {
        if (await repository.TestCodeExistsAsync(command.Code, cancellationToken))
            return Result<RadiologyTestDto>.Failure(
                new(
                    "Radiology.CodeConflict",
                    "Radiology test code already exists.",
                    ErrorType.Conflict
                )
            );
        var test = new RadiologyTest(
            tenant.TenantId,
            command.Code,
            command.Name,
            command.Modality,
            command.Price
        );
        await repository.AddTestAsync(test, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return Result<RadiologyTestDto>.Success(RadiologyTestDto.From(test));
    }
}

public sealed record GetRadiologyTestQuery(Guid Id) : IQuery<Result<RadiologyTestDto>>;

public sealed class GetRadiologyTestQueryHandler(IRadiologyRepository repository)
    : IRequestHandler<GetRadiologyTestQuery, Result<RadiologyTestDto>>
{
    public async Task<Result<RadiologyTestDto>> Handle(
        GetRadiologyTestQuery query,
        CancellationToken cancellationToken
    )
    {
        var test = await repository.GetTestAsync(query.Id, cancellationToken);
        return test is null
            ? Result<RadiologyTestDto>.Failure(
                new("Radiology.TestNotFound", "Radiology test was not found.", ErrorType.NotFound)
            )
            : Result<RadiologyTestDto>.Success(RadiologyTestDto.From(test));
    }
}

public sealed record SearchRadiologyTestsQuery(string? Search, int PageNumber, int PageSize)
    : IQuery<PagedResult<RadiologyTestDto>>;

public sealed class SearchRadiologyTestsQueryValidator
    : AbstractValidator<SearchRadiologyTestsQuery>
{
    public SearchRadiologyTestsQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class SearchRadiologyTestsQueryHandler(IRadiologyRepository repository)
    : IRequestHandler<SearchRadiologyTestsQuery, PagedResult<RadiologyTestDto>>
{
    public async Task<PagedResult<RadiologyTestDto>> Handle(
        SearchRadiologyTestsQuery query,
        CancellationToken cancellationToken
    )
    {
        var page = await repository.SearchTestsAsync(
            query.Search,
            query.PageNumber,
            query.PageSize,
            cancellationToken
        );
        return new(
            page.Items.Select(RadiologyTestDto.From).ToList(),
            page.PageNumber,
            page.PageSize,
            page.TotalCount
        );
    }
}

public sealed record GetRadiologyResultHistoryQuery(Guid OrderId)
    : IQuery<IReadOnlyList<RadiologyResultDto>>;

public sealed class GetRadiologyResultHistoryQueryHandler(IRadiologyRepository repository)
    : IRequestHandler<GetRadiologyResultHistoryQuery, IReadOnlyList<RadiologyResultDto>>
{
    public async Task<IReadOnlyList<RadiologyResultDto>> Handle(
        GetRadiologyResultHistoryQuery query,
        CancellationToken cancellationToken
    ) =>
        (await repository.GetResultsAsync(query.OrderId, cancellationToken))
            .Select(RadiologyResultDto.From)
            .ToList();
}
