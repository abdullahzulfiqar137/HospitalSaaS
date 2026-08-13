using HospitalSaaS.Domain.Exceptions;
using HospitalSaaS.Domain.Payments;

namespace HospitalSaaS.Domain.Tests;

public sealed class PaymentTests
{
    [Fact]
    public void Refund_cannot_exceed_payment_amount()
    {
        var payment = new Payment(Guid.NewGuid(), Guid.NewGuid(), "PAY-1", 100m, "Cash");
        Assert.Throws<DomainException>(() => payment.Refund(101m));
    }

    [Fact]
    public void Full_refund_marks_payment_refunded()
    {
        var payment = new Payment(Guid.NewGuid(), Guid.NewGuid(), "PAY-1", 100m, "Cash");
        payment.Refund(100m);
        Assert.Equal(PaymentStatus.Refunded, payment.Status);
    }
}
