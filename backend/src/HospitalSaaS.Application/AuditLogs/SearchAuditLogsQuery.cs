using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.AuditLogs;

namespace HospitalSaaS.Application.AuditLogs;

public sealed record AuditLogDto(
    Guid Id,
    Guid? UserId,
    string Action,
    string EntityName,
    string? EntityId,
    string? CorrelationId,
    string? OldValues,
    string? NewValues,
    string? IpAddress,
    DateTime OccurredAtUtc
)
{
    public static AuditLogDto From(AuditLog x) =>
        new(
            x.Id,
            x.UserId,
            x.Action,
            x.EntityName,
            x.EntityId,
            x.CorrelationId,
            x.OldValues,
            x.NewValues,
            x.IpAddress,
            x.OccurredAtUtc
        );
}

public sealed record SearchAuditLogsQuery(
    string? EntityName,
    Guid? UserId,
    int PageNumber,
    int PageSize
) : IQuery<PagedResult<AuditLogDto>>;

public sealed class SearchAuditLogsQueryValidator : AbstractValidator<SearchAuditLogsQuery>
{
    public SearchAuditLogsQueryValidator()
    {
        RuleFor(x => x.EntityName).MaximumLength(200);
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class SearchAuditLogsQueryHandler(IReportingRepository r)
    : IRequestHandler<SearchAuditLogsQuery, PagedResult<AuditLogDto>>
{
    public async Task<PagedResult<AuditLogDto>> Handle(SearchAuditLogsQuery x, CancellationToken ct)
    {
        var p = await r.SearchAuditLogsAsync(x.EntityName, x.UserId, x.PageNumber, x.PageSize, ct);
        return new(
            p.Items.Select(AuditLogDto.From).ToList(),
            p.PageNumber,
            p.PageSize,
            p.TotalCount
        );
    }
}
