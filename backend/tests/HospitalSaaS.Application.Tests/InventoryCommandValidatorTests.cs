using HospitalSaaS.Application.Inventory;

namespace HospitalSaaS.Application.Tests;

public sealed class InventoryCommandValidatorTests
{
    [Fact]
    public async Task CreateItem_Rejects_Missing_Fields_And_Negative_Minimum()
    {
        var command = new CreateInventoryItemCommand("", "", "", -1);
        var result = await new CreateInventoryItemCommandValidator().ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Equal(4, result.Errors.Count);
    }

    [Fact]
    public async Task Adjustment_Rejects_Zero_Quantity_And_Missing_Reason()
    {
        var command = new AdjustInventoryStockCommand(Guid.NewGuid(), 0, "");
        var result = await new AdjustInventoryStockCommandValidator().ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.Quantity));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.Reason));
    }
}
