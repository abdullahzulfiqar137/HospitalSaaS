using HospitalSaaS.Application.Authentication;

namespace HospitalSaaS.Application.Tests;

public sealed class AuthenticationCommandValidatorTests
{
    [Fact]
    public async Task Login_Requires_Valid_Email_And_Password()
    {
        var result = await new LoginCommandValidator().ValidateAsync(
            new LoginCommand("invalid", "")
        );

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(LoginCommand.Email));
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == nameof(LoginCommand.Password)
        );
    }

    [Fact]
    public async Task ChangePassword_Requires_New_Different_Strong_Password()
    {
        var command = new ChangePasswordCommand("same", "same");
        var result = await new ChangePasswordCommandValidator().ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(command.NewPassword));
    }

    [Fact]
    public async Task Refresh_Requires_A_Token()
    {
        var result = await new RefreshTokenCommandValidator().ValidateAsync(
            new RefreshTokenCommand("")
        );

        Assert.False(result.IsValid);
    }
}
