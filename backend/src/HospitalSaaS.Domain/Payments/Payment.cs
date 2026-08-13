using HospitalSaaS.Domain.Common;
using HospitalSaaS.Domain.Exceptions;

namespace HospitalSaaS.Domain.Payments;

public sealed class Payment : TenantEntity
{
    private Payment() { }

    public Payment(Guid tenantId, Guid invoiceId, string number, decimal amount, string method)
        : base(tenantId)
    {
        if (amount <= 0)
            throw new DomainException("Payment amount must be positive.");
        InvoiceId = invoiceId;
        PaymentNumber = number;
        Amount = amount;
        Method = method;
        PaidAtUtc = DateTime.UtcNow;
        Status = PaymentStatus.Completed;
    }

    public Guid InvoiceId { get; private set; }
    public string PaymentNumber { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public string Method { get; private set; } = string.Empty;
    public DateTime PaidAtUtc { get; private set; }
    public PaymentStatus Status { get; private set; }
    public decimal RefundedAmount { get; private set; }

    public void Refund(decimal amount)
    {
        if (amount <= 0 || RefundedAmount + amount > Amount)
            throw new DomainException("Refund amount is invalid.");
        RefundedAmount += amount;
        Status =
            RefundedAmount == Amount ? PaymentStatus.Refunded : PaymentStatus.PartiallyRefunded;
    }
}

public enum PaymentStatus
{
    Completed,
    PartiallyRefunded,
    Refunded,
    Failed,
}
