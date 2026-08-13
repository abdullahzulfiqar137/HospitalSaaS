using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;

namespace HospitalSaaS.Application.Authentication;

public sealed record ChangePasswordCommand(string CurrentPassword, string NewPassword)
    : ICommand<Result>;

public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty().MaximumLength(128);
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(12).MaximumLength(128);
        RuleFor(x => x.NewPassword)
            .NotEqual(x => x.CurrentPassword)
            .WithMessage("New password must differ from the current password.");
    }
}

public sealed class ChangePasswordCommandHandler(IAuthenticationService authentication)
    : IRequestHandler<ChangePasswordCommand, Result>
{
    public Task<Result> Handle(
        ChangePasswordCommand command,
        CancellationToken cancellationToken
    ) =>
        authentication.ChangePasswordAsync(
            command.CurrentPassword,
            command.NewPassword,
            cancellationToken
        );
}
