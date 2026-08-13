using HospitalSaaS.Application.Laboratory;

namespace HospitalSaaS.Application.Tests;

public sealed class LaboratoryCommandValidatorTests
{
    [Fact]
    public async Task CreateOrder_Requires_Patient_Doctor_And_Tests()
    {
        var command = new CreateLabOrderCommand(Guid.Empty, Guid.Empty, []);
        var result = await new CreateLabOrderCommandValidator().ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Equal(3, result.Errors.Count);
    }

    [Fact]
    public async Task RecordResult_Requires_Order_Item_And_Result()
    {
        var command = new RecordLabResultCommand(Guid.Empty, Guid.Empty, "", null, null);
        var result = await new RecordLabResultCommandValidator().ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.Result));
    }
}
