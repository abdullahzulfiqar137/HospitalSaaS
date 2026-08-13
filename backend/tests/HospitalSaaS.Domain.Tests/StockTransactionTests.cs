using HospitalSaaS.Domain.Exceptions;
using HospitalSaaS.Domain.Pharmacy;

namespace HospitalSaaS.Domain.Tests;

public sealed class StockTransactionTests
{
    [Fact]
    public void Zero_quantity_stock_transaction_is_rejected() =>
        Assert.Throws<DomainException>(() =>
            new StockTransaction(
                Guid.NewGuid(),
                Guid.NewGuid(),
                StockTransactionType.Adjustment,
                0,
                null
            )
        );
}
