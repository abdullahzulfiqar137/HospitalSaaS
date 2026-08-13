using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Subscriptions;

namespace HospitalSaaS.Application.Subscriptions;

public sealed record SubscriptionDto(
    Guid Id,
    string Plan,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc,
    SubscriptionStatus Status
)
{
    public static SubscriptionDto From(TenantSubscription x) =>
        new(x.Id, x.Plan, x.StartsAtUtc, x.EndsAtUtc, x.Status);
}

public sealed record CreateSubscriptionCommand(
    string Plan,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc
) : ICommand<Result<SubscriptionDto>>;

public sealed class CreateSubscriptionCommandValidator
    : AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        RuleFor(x => x.Plan).NotEmpty().MaximumLength(100);
        RuleFor(x => x.StartsAtUtc).Must(x => x.Kind == DateTimeKind.Utc);
        RuleFor(x => x.EndsAtUtc)
            .Must(x => x.Kind == DateTimeKind.Utc)
            .GreaterThan(x => x.StartsAtUtc);
    }
}

public sealed class CreateSubscriptionCommandHandler(ISubscriptionRepository r, ITenantContext t)
    : IRequestHandler<CreateSubscriptionCommand, Result<SubscriptionDto>>
{
    public async Task<Result<SubscriptionDto>> Handle(
        CreateSubscriptionCommand x,
        CancellationToken ct
    )
    {
        var e = new TenantSubscription(t.TenantId, x.Plan.Trim(), x.StartsAtUtc, x.EndsAtUtc);
        await r.AddAsync(e, ct);
        await r.SaveChangesAsync(ct);
        return Result<SubscriptionDto>.Success(SubscriptionDto.From(e));
    }
}

public sealed record SearchSubscriptionsQuery(int PageNumber, int PageSize)
    : IQuery<PagedResult<SubscriptionDto>>;

public sealed class SearchSubscriptionsQueryValidator : AbstractValidator<SearchSubscriptionsQuery>
{
    public SearchSubscriptionsQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class SearchSubscriptionsQueryHandler(ISubscriptionRepository r)
    : IRequestHandler<SearchSubscriptionsQuery, PagedResult<SubscriptionDto>>
{
    public async Task<PagedResult<SubscriptionDto>> Handle(
        SearchSubscriptionsQuery x,
        CancellationToken ct
    )
    {
        var p = await r.SearchAsync(x.PageNumber, x.PageSize, ct);
        return new(
            p.Items.Select(SubscriptionDto.From).ToList(),
            p.PageNumber,
            p.PageSize,
            p.TotalCount
        );
    }
}
