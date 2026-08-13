using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Notifications;

namespace HospitalSaaS.Application.Notifications;

public sealed record NotificationDto(
    Guid Id,
    Guid UserId,
    string Subject,
    string Channel,
    NotificationStatus Status,
    DateTime? SentAtUtc
)
{
    public static NotificationDto From(Notification x) =>
        new(x.Id, x.UserId, x.Subject, x.Channel, x.Status, x.SentAtUtc);
}

public sealed record CreateNotificationCommand(
    Guid UserId,
    string Subject,
    string Body,
    string Channel
) : ICommand<Result<NotificationDto>>;

public sealed class CreateNotificationCommandValidator
    : AbstractValidator<CreateNotificationCommand>
{
    private static readonly string[] Channels = ["Email", "Sms", "InApp"];

    public CreateNotificationCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Body).NotEmpty().MaximumLength(8000);
        RuleFor(x => x.Channel)
            .Must(x => Channels.Contains(x, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Channel must be Email, Sms, or InApp.");
    }
}

public sealed class CreateNotificationCommandHandler(INotificationRepository r, ITenantContext t)
    : IRequestHandler<CreateNotificationCommand, Result<NotificationDto>>
{
    public async Task<Result<NotificationDto>> Handle(
        CreateNotificationCommand x,
        CancellationToken ct
    )
    {
        var e = new Notification(
            t.TenantId,
            x.UserId,
            x.Subject.Trim(),
            x.Body.Trim(),
            x.Channel.Trim()
        );
        await r.AddAsync(e, ct);
        await r.SaveChangesAsync(ct);
        return Result<NotificationDto>.Success(NotificationDto.From(e));
    }
}

public sealed record SearchNotificationsQuery(
    Guid? UserId,
    NotificationStatus? Status,
    int PageNumber,
    int PageSize
) : IQuery<PagedResult<NotificationDto>>;

public sealed class SearchNotificationsQueryValidator : AbstractValidator<SearchNotificationsQuery>
{
    public SearchNotificationsQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class SearchNotificationsQueryHandler(INotificationRepository r)
    : IRequestHandler<SearchNotificationsQuery, PagedResult<NotificationDto>>
{
    public async Task<PagedResult<NotificationDto>> Handle(
        SearchNotificationsQuery x,
        CancellationToken ct
    )
    {
        var p = await r.SearchAsync(x.UserId, x.Status, x.PageNumber, x.PageSize, ct);
        return new(
            p.Items.Select(NotificationDto.From).ToList(),
            p.PageNumber,
            p.PageSize,
            p.TotalCount
        );
    }
}
