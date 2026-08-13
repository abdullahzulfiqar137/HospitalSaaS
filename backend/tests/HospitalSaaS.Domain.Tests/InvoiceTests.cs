using HospitalSaaS.Domain.Billing;

namespace HospitalSaaS.Domain.Tests;

public sealed class InvoiceTests
{
    [Fact]
    public void Total_includes_discount_and_tax()
    {
        var invoice = new Invoice(Guid.NewGuid(), "INV-1", Guid.NewGuid());
        invoice.AddItem("Procedure", 2, 100, 10, 5);
        Assert.Equal(195, invoice.Total);
    }
}
