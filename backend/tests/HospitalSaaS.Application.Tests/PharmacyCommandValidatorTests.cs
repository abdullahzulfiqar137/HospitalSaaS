using HospitalSaaS.Application.Pharmacy;
using HospitalSaaS.Domain.Pharmacy;

namespace HospitalSaaS.Application.Tests;

public sealed class PharmacyCommandValidatorTests
{
    [Fact]
    public async Task CreateMedicine_Rejects_Missing_Fields_And_Negative_Prices()
    {
        var command = new CreateMedicineCommand("", "", null, "", "", null, null, -1, -1, -1);
        var result = await new CreateMedicineCommandValidator().ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.Name));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.SalePrice));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.MinimumStockLevel));
    }

    [Fact]
    public async Task StockTransaction_Rejects_Zero_Quantity()
    {
        var command = new RecordMedicineStockCommand(
            Guid.NewGuid(),
            StockTransactionType.Adjustment,
            0,
            null
        );
        var result = await new RecordMedicineStockCommandValidator().ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.Quantity));
    }
}
