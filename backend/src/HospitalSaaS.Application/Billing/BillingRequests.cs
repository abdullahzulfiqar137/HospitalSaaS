using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Billing;
using HospitalSaaS.Domain.Payments;

namespace HospitalSaaS.Application.Billing;

public sealed record InvoiceLineDto(
    Guid Id,
    string Description,
    decimal Quantity,
    decimal UnitPrice,
    decimal Discount,
    decimal Tax
);

public sealed record InvoiceDto(
    Guid Id,
    string InvoiceNumber,
    Guid PatientId,
    DateTime IssuedAtUtc,
    InvoiceStatus Status,
    decimal Subtotal,
    decimal Discount,
    decimal Tax,
    decimal Total,
    IReadOnlyList<InvoiceLineDto> Items
)
{
    public static InvoiceDto From(Invoice x) =>
        new(
            x.Id,
            x.InvoiceNumber,
            x.PatientId,
            x.IssuedAtUtc,
            x.Status,
            x.Subtotal,
            x.Discount,
            x.Tax,
            x.Total,
            x.Items.Select(i => new InvoiceLineDto(
                    i.Id,
                    i.Description,
                    i.Quantity,
                    i.UnitPrice,
                    i.Discount,
                    i.Tax
                ))
                .ToList()
        );
}

public sealed record PaymentDto(
    Guid Id,
    Guid InvoiceId,
    string PaymentNumber,
    decimal Amount,
    string Method,
    DateTime PaidAtUtc,
    PaymentStatus Status,
    decimal RefundedAmount
)
{
    public static PaymentDto From(Payment x) =>
        new(
            x.Id,
            x.InvoiceId,
            x.PaymentNumber,
            x.Amount,
            x.Method,
            x.PaidAtUtc,
            x.Status,
            x.RefundedAmount
        );
}

public sealed record InvoiceItemInput(
    string Description,
    decimal Quantity,
    decimal UnitPrice,
    decimal Discount,
    decimal Tax
);

public sealed record CreateInvoiceCommand(Guid PatientId, IReadOnlyList<InvoiceItemInput> Items)
    : ICommand<Result<InvoiceDto>>;

public sealed class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items)
            .ChildRules(i =>
            {
                i.RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
                i.RuleFor(x => x.Quantity).GreaterThan(0);
                i.RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
                i.RuleFor(x => x.Discount).GreaterThanOrEqualTo(0);
                i.RuleFor(x => x.Tax).GreaterThanOrEqualTo(0);
            });
    }
}

public sealed class CreateInvoiceCommandHandler(IBillingRepository r, ITenantContext t)
    : IRequestHandler<CreateInvoiceCommand, Result<InvoiceDto>>
{
    public async Task<Result<InvoiceDto>> Handle(CreateInvoiceCommand x, CancellationToken ct)
    {
        var y = DateTime.UtcNow.Year;
        var n = await r.CountInvoicesAsync(y, ct) + 1;
        var no = $"INV-{y}-{n:000000}";
        while (await r.InvoiceNumberExistsAsync(no, ct))
            no = $"INV-{y}-{++n:000000}";
        var e = new Invoice(t.TenantId, no, x.PatientId);
        foreach (var i in x.Items)
            e.AddItem(i.Description, i.Quantity, i.UnitPrice, i.Discount, i.Tax);
        e.Issue();
        await r.AddInvoiceAsync(e, ct);
        await r.SaveChangesAsync(ct);
        return Result<InvoiceDto>.Success(InvoiceDto.From(e));
    }
}

public sealed record GetInvoiceQuery(Guid Id) : IQuery<Result<InvoiceDto>>;

public sealed class GetInvoiceQueryHandler(IBillingRepository r)
    : IRequestHandler<GetInvoiceQuery, Result<InvoiceDto>>
{
    public async Task<Result<InvoiceDto>> Handle(GetInvoiceQuery x, CancellationToken ct)
    {
        var e = await r.GetInvoiceAsync(x.Id, false, ct);
        return e is null
            ? Result<InvoiceDto>.Failure(
                new("Billing.NotFound", "Invoice was not found.", ErrorType.NotFound)
            )
            : Result<InvoiceDto>.Success(InvoiceDto.From(e));
    }
}

public sealed record SearchInvoicesQuery(
    Guid? PatientId,
    InvoiceStatus? Status,
    int PageNumber,
    int PageSize
) : IQuery<PagedResult<InvoiceDto>>;

public sealed class SearchInvoicesQueryValidator : AbstractValidator<SearchInvoicesQuery>
{
    public SearchInvoicesQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class SearchInvoicesQueryHandler(IBillingRepository r)
    : IRequestHandler<SearchInvoicesQuery, PagedResult<InvoiceDto>>
{
    public async Task<PagedResult<InvoiceDto>> Handle(SearchInvoicesQuery x, CancellationToken ct)
    {
        var p = await r.SearchInvoicesAsync(x.PatientId, x.Status, x.PageNumber, x.PageSize, ct);
        return new(
            p.Items.Select(InvoiceDto.From).ToList(),
            p.PageNumber,
            p.PageSize,
            p.TotalCount
        );
    }
}
