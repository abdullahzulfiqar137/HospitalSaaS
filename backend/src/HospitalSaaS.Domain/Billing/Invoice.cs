using HospitalSaaS.Domain.Common;
using HospitalSaaS.Domain.Exceptions;

namespace HospitalSaaS.Domain.Billing;

public sealed class Invoice : AggregateRoot
{
    private readonly List<InvoiceItem> _items = [];

    private Invoice() { }

    public Invoice(Guid tenantId, string number, Guid patientId)
        : base(tenantId)
    {
        InvoiceNumber = number;
        PatientId = patientId;
        IssuedAtUtc = DateTime.UtcNow;
        Status = InvoiceStatus.Draft;
    }

    public string InvoiceNumber { get; private set; } = string.Empty;
    public Guid PatientId { get; private set; }
    public DateTime IssuedAtUtc { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public IReadOnlyCollection<InvoiceItem> Items => _items;
    public decimal Subtotal => _items.Sum(x => x.Quantity * x.UnitPrice);
    public decimal Discount => _items.Sum(x => x.Discount);
    public decimal Tax => _items.Sum(x => x.Tax);
    public decimal Total => Subtotal - Discount + Tax;

    public void AddItem(
        string description,
        decimal quantity,
        decimal unitPrice,
        decimal discount = 0,
        decimal tax = 0
    )
    {
        if (quantity <= 0 || unitPrice < 0)
            throw new DomainException("Invoice quantity and price are invalid.");
        _items.Add(new(Id, description, quantity, unitPrice, discount, tax));
    }

    public void Issue()
    {
        if (_items.Count == 0)
            throw new DomainException("Invoice must contain an item.");
        Status = InvoiceStatus.Issued;
    }

    public void MarkPaid() => Status = InvoiceStatus.Paid;
}

public enum InvoiceStatus
{
    Draft,
    Issued,
    PartiallyPaid,
    Paid,
    Cancelled,
}
