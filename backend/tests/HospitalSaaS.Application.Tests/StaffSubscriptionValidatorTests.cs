using HospitalSaaS.Application.Staff;
using HospitalSaaS.Application.Subscriptions;

namespace HospitalSaaS.Application.Tests;

public sealed class StaffSubscriptionValidatorTests
{
    [Fact]
    public async Task Staff_Requires_Employee_Identity_And_Title()
    {
        var command = new CreateStaffCommand("", "", "", "", null);
        var result = await new CreateStaffCommandValidator().ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Equal(4, result.Errors.Count);
    }

    [Fact]
    public async Task Subscription_Requires_Utc_Ordered_Date_Range()
    {
        var start = DateTime.SpecifyKind(new DateTime(2026, 2, 1), DateTimeKind.Local);
        var end = DateTime.SpecifyKind(new DateTime(2026, 1, 1), DateTimeKind.Local);
        var result = await new CreateSubscriptionCommandValidator().ValidateAsync(
            new CreateSubscriptionCommand("Plan", start, end)
        );
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 2);
    }
}
