using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Insurance;

namespace HospitalSaaS.Application.Insurance;

public sealed record InsurancePolicyDto(
    Guid Id,
    Guid PatientId,
    string Provider,
    string PolicyNumber,
    DateOnly ValidFrom,
    DateOnly ValidTo
)
{
    public static InsurancePolicyDto From(InsurancePolicy x) =>
        new(x.Id, x.PatientId, x.Provider, x.PolicyNumber, x.ValidFrom, x.ValidTo);
}

public sealed record CreateInsurancePolicyCommand(
    Guid PatientId,
    string Provider,
    string PolicyNumber,
    DateOnly ValidFrom,
    DateOnly ValidTo
) : ICommand<Result<InsurancePolicyDto>>;

public sealed class CreateInsurancePolicyCommandValidator
    : AbstractValidator<CreateInsurancePolicyCommand>
{
    public CreateInsurancePolicyCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.Provider).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PolicyNumber).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ValidTo).GreaterThanOrEqualTo(x => x.ValidFrom);
    }
}

public sealed class CreateInsurancePolicyCommandHandler(IInsuranceRepository r, ITenantContext t)
    : IRequestHandler<CreateInsurancePolicyCommand, Result<InsurancePolicyDto>>
{
    public async Task<Result<InsurancePolicyDto>> Handle(
        CreateInsurancePolicyCommand x,
        CancellationToken ct
    )
    {
        if (await r.PolicyNumberExistsAsync(x.PolicyNumber, ct))
            return Result<InsurancePolicyDto>.Failure(
                new("Insurance.PolicyConflict", "Policy number already exists.", ErrorType.Conflict)
            );
        var e = new InsurancePolicy(
            t.TenantId,
            x.PatientId,
            x.Provider.Trim(),
            x.PolicyNumber.Trim(),
            x.ValidFrom,
            x.ValidTo
        );
        await r.AddAsync(e, ct);
        await r.SaveChangesAsync(ct);
        return Result<InsurancePolicyDto>.Success(InsurancePolicyDto.From(e));
    }
}

public sealed record SearchInsurancePoliciesQuery(Guid? PatientId, int PageNumber, int PageSize)
    : IQuery<PagedResult<InsurancePolicyDto>>;

public sealed class SearchInsurancePoliciesQueryValidator
    : AbstractValidator<SearchInsurancePoliciesQuery>
{
    public SearchInsurancePoliciesQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class SearchInsurancePoliciesQueryHandler(IInsuranceRepository r)
    : IRequestHandler<SearchInsurancePoliciesQuery, PagedResult<InsurancePolicyDto>>
{
    public async Task<PagedResult<InsurancePolicyDto>> Handle(
        SearchInsurancePoliciesQuery x,
        CancellationToken ct
    )
    {
        var p = await r.SearchAsync(x.PatientId, x.PageNumber, x.PageSize, ct);
        return new(
            p.Items.Select(InsurancePolicyDto.From).ToList(),
            p.PageNumber,
            p.PageSize,
            p.TotalCount
        );
    }
}
