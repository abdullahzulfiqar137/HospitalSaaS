namespace HospitalSaaS.Contracts.Billing;

public sealed record CreateInvoiceRequest(Guid PatientId, IReadOnlyList<InvoiceItemRequest> Items);

public sealed record InvoiceItemRequest(
    string Description,
    decimal Quantity,
    decimal UnitPrice,
    decimal Discount,
    decimal Tax
);

public sealed record RecordPaymentRequest(decimal Amount, string Method);

public sealed record RefundPaymentRequest(decimal Amount);
