using HospitalSaaS.Application.Administration;

namespace HospitalSaaS.Application.Tests;

public sealed class AdministrationCommandValidatorTests
{
    [Theory]
    [InlineData("Has Spaces")]
    [InlineData("UPPERCASE")]
    [InlineData("ends-")]
    public async Task Tenant_Rejects_Invalid_Slug(string slug)
    {
        var result = await new CreateTenantCommandValidator().ValidateAsync(
            new CreateTenantCommand("Hospital", slug)
        );
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(CreateTenantCommand.Slug));
    }

    [Fact]
    public async Task User_Requires_Strong_Minimum_Password_Length()
    {
        var command = new CreateUserCommand("user@example.com", "short", []);
        var result = await new CreateUserCommandValidator().ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.Password));
    }

    [Fact]
    public async Task Role_Requires_A_Name()
    {
        var result = await new CreateRoleCommandValidator().ValidateAsync(
            new CreateRoleCommand("", [])
        );
        Assert.False(result.IsValid);
    }
}
