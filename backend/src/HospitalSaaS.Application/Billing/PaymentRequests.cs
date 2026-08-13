using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;

namespace HospitalSaaS.Application.Billing;

public sealed record RecordPaymentCommand(Guid InvoiceId, decimal Amount, string Method)
    : ICommand<Result<PaymentDto>>;

public sealed class RecordPaymentCommandValidator : AbstractValidator<RecordPaymentCommand>
{
    public RecordPaymentCommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Method).NotEmpty().MaximumLength(100);
    }
}

public sealed class RecordPaymentCommandHandler(IBillingRepository r, ITenantContext t)
    : IRequestHandler<RecordPaymentCommand, Result<PaymentDto>>
{
    public async Task<Result<PaymentDto>> Handle(RecordPaymentCommand x, CancellationToken ct)
    {
        var invoice = await r.GetInvoiceAsync(x.InvoiceId, true, ct);
        if (invoice is null)
            return Result<PaymentDto>.Failure(
                new("Billing.NotFound", "Invoice was not found.", ErrorType.NotFound)
            );
        var paid = await r.GetNetPaidAsync(x.InvoiceId, ct);
        if (paid + x.Amount > invoice.Total)
            return Result<PaymentDto>.Failure(
                new(
                    "Payments.Overpayment",
                    "Payment exceeds outstanding balance.",
                    ErrorType.Conflict
                )
            );
        var y = DateTime.UtcNow.Year;
        var n = await r.CountPaymentsAsync(y, ct) + 1;
        var no = $"PAY-{y}-{n:000000}";
        while (await r.PaymentNumberExistsAsync(no, ct))
            no = $"PAY-{y}-{++n:000000}";
        var e = new HospitalSaaS.Domain.Payments.Payment(
            t.TenantId,
            x.InvoiceId,
            no,
            x.Amount,
            x.Method.Trim()
        );
        await r.AddPaymentAsync(e, ct);
        if (paid + x.Amount == invoice.Total)
            invoice.MarkPaid();
        await r.SaveChangesAsync(ct);
        return Result<PaymentDto>.Success(PaymentDto.From(e));
    }
}

public sealed record RefundPaymentCommand(Guid Id, decimal Amount) : ICommand<Result<PaymentDto>>;

public sealed class RefundPaymentCommandValidator : AbstractValidator<RefundPaymentCommand>
{
    public RefundPaymentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}

public sealed class RefundPaymentCommandHandler(IBillingRepository r)
    : IRequestHandler<RefundPaymentCommand, Result<PaymentDto>>
{
    public async Task<Result<PaymentDto>> Handle(RefundPaymentCommand x, CancellationToken ct)
    {
        var e = await r.GetPaymentAsync(x.Id, true, ct);
        if (e is null)
            return Result<PaymentDto>.Failure(
                new("Payments.NotFound", "Payment was not found.", ErrorType.NotFound)
            );
        e.Refund(x.Amount);
        await r.SaveChangesAsync(ct);
        return Result<PaymentDto>.Success(PaymentDto.From(e));
    }
}

public sealed record SearchPaymentsQuery(Guid? InvoiceId, int PageNumber, int PageSize)
    : IQuery<PagedResult<PaymentDto>>;

public sealed class SearchPaymentsQueryValidator : AbstractValidator<SearchPaymentsQuery>
{
    public SearchPaymentsQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class SearchPaymentsQueryHandler(IBillingRepository r)
    : IRequestHandler<SearchPaymentsQuery, PagedResult<PaymentDto>>
{
    public async Task<PagedResult<PaymentDto>> Handle(SearchPaymentsQuery x, CancellationToken ct)
    {
        var p = await r.SearchPaymentsAsync(x.InvoiceId, x.PageNumber, x.PageSize, ct);
        return new(
            p.Items.Select(PaymentDto.From).ToList(),
            p.PageNumber,
            p.PageSize,
            p.TotalCount
        );
    }
}
