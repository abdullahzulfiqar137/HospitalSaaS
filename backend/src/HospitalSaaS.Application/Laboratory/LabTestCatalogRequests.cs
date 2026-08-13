using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Laboratory;

namespace HospitalSaaS.Application.Laboratory;

public sealed record CreateLabTestCommand(
    string Name,
    string Code,
    string Category,
    decimal Price,
    string? NormalRange,
    string? Unit
) : ICommand<Result<LabTestDto>>;

public sealed class CreateLabTestCommandValidator : AbstractValidator<CreateLabTestCommand>
{
    public CreateLabTestCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.NormalRange).MaximumLength(200);
        RuleFor(x => x.Unit).MaximumLength(100);
    }
}

public sealed class CreateLabTestCommandHandler(
    ILaboratoryRepository repository,
    ITenantContext tenant
) : IRequestHandler<CreateLabTestCommand, Result<LabTestDto>>
{
    public async Task<Result<LabTestDto>> Handle(
        CreateLabTestCommand command,
        CancellationToken cancellationToken
    )
    {
        if (await repository.TestCodeExistsAsync(command.Code, cancellationToken))
            return Result<LabTestDto>.Failure(
                new("Lab.CodeConflict", "Lab test code already exists.", ErrorType.Conflict)
            );
        var test = new LabTest(
            tenant.TenantId,
            command.Name,
            command.Code,
            command.Category,
            command.Price,
            command.NormalRange,
            command.Unit
        );
        await repository.AddTestAsync(test, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return Result<LabTestDto>.Success(LabTestDto.From(test));
    }
}

public sealed record GetLabTestQuery(Guid Id) : IQuery<Result<LabTestDto>>;

public sealed class GetLabTestQueryHandler(ILaboratoryRepository repository)
    : IRequestHandler<GetLabTestQuery, Result<LabTestDto>>
{
    public async Task<Result<LabTestDto>> Handle(GetLabTestQuery query, CancellationToken ct)
    {
        var test = await repository.GetTestAsync(query.Id, ct);
        return test is null
            ? Result<LabTestDto>.Failure(
                new("Lab.TestNotFound", "Lab test was not found.", ErrorType.NotFound)
            )
            : Result<LabTestDto>.Success(LabTestDto.From(test));
    }
}

public sealed record SearchLabTestsQuery(string? Search, int PageNumber, int PageSize)
    : IQuery<PagedResult<LabTestDto>>;

public sealed class SearchLabTestsQueryValidator : AbstractValidator<SearchLabTestsQuery>
{
    public SearchLabTestsQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class SearchLabTestsQueryHandler(ILaboratoryRepository repository)
    : IRequestHandler<SearchLabTestsQuery, PagedResult<LabTestDto>>
{
    public async Task<PagedResult<LabTestDto>> Handle(
        SearchLabTestsQuery query,
        CancellationToken ct
    )
    {
        var page = await repository.SearchTestsAsync(
            query.Search,
            query.PageNumber,
            query.PageSize,
            ct
        );
        return new(
            page.Items.Select(LabTestDto.From).ToList(),
            page.PageNumber,
            page.PageSize,
            page.TotalCount
        );
    }
}
