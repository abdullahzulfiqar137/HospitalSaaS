using FluentValidation;

namespace HospitalSaaS.Application.Admissions;

public sealed class CreateWardCommandValidator : AbstractValidator<CreateWardCommand>
{
    public CreateWardCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(30);
    }
}

public sealed class CreateRoomCommandValidator : AbstractValidator<CreateRoomCommand>
{
    public CreateRoomCommandValidator()
    {
        RuleFor(x => x.WardId).NotEmpty();
        RuleFor(x => x.Number).NotEmpty().MaximumLength(30);
    }
}

public sealed class CreateBedCommandValidator : AbstractValidator<CreateBedCommand>
{
    public CreateBedCommandValidator()
    {
        RuleFor(x => x.RoomId).NotEmpty();
        RuleFor(x => x.Number).NotEmpty().MaximumLength(30);
    }
}
