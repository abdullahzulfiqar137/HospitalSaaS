using HospitalSaaS.Application.Billing;

namespace HospitalSaaS.Application.Tests;

public sealed class BillingCommandValidatorTests
{
    [Fact]
    public async Task Invoice_Requires_Patient_And_Valid_Line_Items()
    {
        var command = new CreateInvoiceCommand(
            Guid.Empty,
            [new InvoiceItemInput("", 0, -1, -1, -1)]
        );
        var result = await new CreateInvoiceCommandValidator().ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.PatientId));
        Assert.Contains(
            result.Errors,
            x => x.PropertyName.Contains(nameof(InvoiceItemInput.Quantity))
        );
    }

    [Fact]
    public async Task Payment_Requires_Positive_Amount_And_Method()
    {
        var command = new RecordPaymentCommand(Guid.Empty, 0, "");
        var result = await new RecordPaymentCommandValidator().ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Equal(3, result.Errors.Count);
    }

    [Fact]
    public async Task Refund_Requires_Positive_Amount()
    {
        var command = new RefundPaymentCommand(Guid.NewGuid(), 0);
        var result = await new RefundPaymentCommandValidator().ValidateAsync(command);
        Assert.False(result.IsValid);
    }
}
