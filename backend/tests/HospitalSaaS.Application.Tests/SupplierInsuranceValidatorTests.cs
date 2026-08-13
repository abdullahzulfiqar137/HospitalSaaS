using HospitalSaaS.Application.Insurance;
using HospitalSaaS.Application.Suppliers;

namespace HospitalSaaS.Application.Tests;

public sealed class SupplierInsuranceValidatorTests
{
    [Fact]
    public async Task Supplier_Requires_Name_And_Valid_Email()
    {
        var command = new CreateSupplierCommand("", null, null, "invalid", null);
        var result = await new CreateSupplierCommandValidator().ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);
    }

    [Fact]
    public async Task Policy_Requires_Valid_Date_Range()
    {
        var command = new CreateInsurancePolicyCommand(
            Guid.NewGuid(),
            "Provider",
            "POL-1",
            new DateOnly(2026, 12, 1),
            new DateOnly(2026, 1, 1)
        );
        var result = await new CreateInsurancePolicyCommandValidator().ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.ValidTo));
    }
}
