using HospitalSaaS.Api.Authorization;
using HospitalSaaS.Application.Billing;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Contracts.Billing;
using HospitalSaaS.Contracts.Common;
using HospitalSaaS.Domain.Billing;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSaaS.Api.Controllers;

[Route("api/v1/billing")]
public sealed class BillingController(IRequestDispatcher dispatcher) : ApiControllerBase
{
    [HttpPost("invoices"), HasPermission("Billing.Create")]
    public async Task<IActionResult> Create(CreateInvoiceRequest request, CancellationToken ct)
    {
        var command = new CreateInvoiceCommand(
            request.PatientId,
            request
                .Items.Select(x => new InvoiceItemInput(
                    x.Description,
                    x.Quantity,
                    x.UnitPrice,
                    x.Discount,
                    x.Tax
                ))
                .ToList()
        );
        var result = await dispatcher.Send<CreateInvoiceCommand, Result<InvoiceDto>>(command, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, Map(result.Value))
            : FromError(result.Error!);
    }

    [HttpGet("invoices/{id:guid}"), HasPermission("Billing.View")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var result = await dispatcher.Send<GetInvoiceQuery, Result<InvoiceDto>>(new(id), ct);
        return result.IsSuccess ? Ok(Map(result.Value!)) : FromError(result.Error!);
    }

    [HttpGet("invoices"), HasPermission("Billing.View")]
    public async Task<IActionResult> Invoices(
        Guid? patientId,
        InvoiceStatus? status,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken ct = default
    )
    {
        var result = await dispatcher.Send<SearchInvoicesQuery, PagedResult<InvoiceDto>>(
            new(patientId, status, pageNumber, pageSize),
            ct
        );
        return Ok(
            new PagedResponse<InvoiceResponse>(
                result.Items.Select(Map).ToList(),
                result.PageNumber,
                result.PageSize,
                result.TotalCount,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            )
        );
    }

    [HttpPost("invoices/{id:guid}/payments"), HasPermission("Payments.Create")]
    public async Task<IActionResult> Pay(
        Guid id,
        RecordPaymentRequest request,
        CancellationToken ct
    )
    {
        var result = await dispatcher.Send<RecordPaymentCommand, Result<PaymentDto>>(
            new(id, request.Amount, request.Method),
            ct
        );
        return result.IsSuccess ? Ok(Map(result.Value!)) : FromError(result.Error!);
    }

    [HttpGet("payments"), HasPermission("Payments.View")]
    public async Task<IActionResult> Payments(
        Guid? invoiceId,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken ct = default
    )
    {
        var result = await dispatcher.Send<SearchPaymentsQuery, PagedResult<PaymentDto>>(
            new(invoiceId, pageNumber, pageSize),
            ct
        );
        return Ok(
            new PagedResponse<PaymentResponse>(
                result.Items.Select(Map).ToList(),
                result.PageNumber,
                result.PageSize,
                result.TotalCount,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            )
        );
    }

    [HttpPost("payments/{id:guid}/refund"), HasPermission("Payments.Refund")]
    public async Task<IActionResult> Refund(
        Guid id,
        RefundPaymentRequest request,
        CancellationToken ct
    )
    {
        var result = await dispatcher.Send<RefundPaymentCommand, Result<PaymentDto>>(
            new(id, request.Amount),
            ct
        );
        return result.IsSuccess ? Ok(Map(result.Value!)) : FromError(result.Error!);
    }

    private static InvoiceResponse Map(InvoiceDto x) =>
        new(
            x.Id,
            x.InvoiceNumber,
            x.PatientId,
            x.IssuedAtUtc,
            x.Status.ToString(),
            x.Subtotal,
            x.Discount,
            x.Tax,
            x.Total,
            x.Items.Select(i => new InvoiceLineResponse(
                    i.Id,
                    i.Description,
                    i.Quantity,
                    i.UnitPrice,
                    i.Discount,
                    i.Tax
                ))
                .ToList()
        );

    private static PaymentResponse Map(PaymentDto x) =>
        new(
            x.Id,
            x.InvoiceId,
            x.PaymentNumber,
            x.Amount,
            x.Method,
            x.PaidAtUtc,
            x.Status.ToString(),
            x.RefundedAmount
        );
}
