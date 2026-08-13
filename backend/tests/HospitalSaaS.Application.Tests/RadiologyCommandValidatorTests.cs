using HospitalSaaS.Application.Radiology;

namespace HospitalSaaS.Application.Tests;

public sealed class RadiologyCommandValidatorTests
{
    [Fact]
    public async Task CreateOrder_Requires_Patient_Doctor_And_Procedure()
    {
        var command = new CreateRadiologyOrderCommand(Guid.Empty, Guid.Empty, "");

        var result = await new CreateRadiologyOrderCommandValidator().ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Equal(3, result.Errors.Count);
    }

    [Fact]
    public async Task RecordReport_Requires_Order_And_Report_Text()
    {
        var command = new RecordRadiologyReportCommand(Guid.Empty, "");

        var result = await new RecordRadiologyReportCommandValidator().ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.Id));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.Report));
    }
}
