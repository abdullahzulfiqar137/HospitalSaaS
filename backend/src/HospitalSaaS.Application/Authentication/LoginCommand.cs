using FluentValidation;
using HospitalSaaS.Application.Authentication.Models;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;

namespace HospitalSaaS.Application.Authentication;

public sealed record LoginCommand(string Email, string Password) : ICommand<Result<AuthTokens>>;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty().MaximumLength(128);
    }
}

public sealed class LoginCommandHandler(IAuthenticationService authentication)
    : IRequestHandler<LoginCommand, Result<AuthTokens>>
{
    public Task<Result<AuthTokens>> Handle(
        LoginCommand command,
        CancellationToken cancellationToken
    ) => authentication.LoginAsync(command.Email, command.Password, cancellationToken);
}
