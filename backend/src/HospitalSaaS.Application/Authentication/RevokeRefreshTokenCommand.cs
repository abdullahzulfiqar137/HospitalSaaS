using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;

namespace HospitalSaaS.Application.Authentication;

public sealed record RevokeRefreshTokenCommand(string RefreshToken) : ICommand<Result>;

public sealed class RevokeRefreshTokenCommandValidator
    : AbstractValidator<RevokeRefreshTokenCommand>
{
    public RevokeRefreshTokenCommandValidator() =>
        RuleFor(x => x.RefreshToken).NotEmpty().MaximumLength(512);
}

public sealed class RevokeRefreshTokenCommandHandler(IAuthenticationService authentication)
    : IRequestHandler<RevokeRefreshTokenCommand, Result>
{
    public Task<Result> Handle(
        RevokeRefreshTokenCommand command,
        CancellationToken cancellationToken
    ) => authentication.RevokeAsync(command.RefreshToken, cancellationToken);
}
