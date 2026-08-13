namespace HospitalSaaS.Contracts.Billing;

public sealed record InvoiceResponse(
    Guid Id,
    string InvoiceNumber,
    Guid PatientId,
    DateTime IssuedAtUtc,
    string Status,
    decimal Subtotal,
    decimal Discount,
    decimal Tax,
    decimal Total,
    IReadOnlyList<InvoiceLineResponse> Items
);

public sealed record InvoiceLineResponse(
    Guid Id,
    string Description,
    decimal Quantity,
    decimal UnitPrice,
    decimal Discount,
    decimal Tax
);

public sealed record PaymentResponse(
    Guid Id,
    Guid InvoiceId,
    string PaymentNumber,
    decimal Amount,
    string Method,
    DateTime PaidAtUtc,
    string Status,
    decimal RefundedAmount
);
