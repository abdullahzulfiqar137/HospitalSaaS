using FluentValidation;
using HospitalSaaS.Application.Authentication.Models;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;

namespace HospitalSaaS.Application.Authentication;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<Result<AuthTokens>>;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator() =>
        RuleFor(x => x.RefreshToken).NotEmpty().MaximumLength(512);
}

public sealed class RefreshTokenCommandHandler(IAuthenticationService authentication)
    : IRequestHandler<RefreshTokenCommand, Result<AuthTokens>>
{
    public Task<Result<AuthTokens>> Handle(
        RefreshTokenCommand command,
        CancellationToken cancellationToken
    ) => authentication.RefreshAsync(command.RefreshToken, cancellationToken);
}
