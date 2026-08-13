using HospitalSaaS.Domain.Common;

namespace HospitalSaaS.Domain.Billing;

public sealed class InvoiceItem : BaseEntity
{
    private InvoiceItem() { }

    internal InvoiceItem(
        Guid invoiceId,
        string description,
        decimal quantity,
        decimal unitPrice,
        decimal discount,
        decimal tax
    )
    {
        InvoiceId = invoiceId;
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Discount = discount;
        Tax = tax;
    }

    public Guid InvoiceId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Discount { get; private set; }
    public decimal Tax { get; private set; }
}
