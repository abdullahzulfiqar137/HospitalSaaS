using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Domain.Radiology;

namespace HospitalSaaS.Application.Radiology;

public sealed record CreateRadiologyOrderCommand(
    Guid PatientId,
    Guid DoctorId,
    string Procedure,
    Guid? RadiologyTestId = null
) : ICommand<Result<RadiologyOrderDto>>;

public sealed class CreateRadiologyOrderCommandValidator
    : AbstractValidator<CreateRadiologyOrderCommand>
{
    public CreateRadiologyOrderCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.Procedure).NotEmpty().MaximumLength(500);
    }
}

public sealed class CreateRadiologyOrderCommandHandler(
    IRadiologyRepository repository,
    ITenantContext tenant
) : IRequestHandler<CreateRadiologyOrderCommand, Result<RadiologyOrderDto>>
{
    public async Task<Result<RadiologyOrderDto>> Handle(
        CreateRadiologyOrderCommand command,
        CancellationToken ct
    )
    {
        RadiologyTest? test = null;
        if (command.RadiologyTestId.HasValue)
        {
            test = await repository.GetTestAsync(command.RadiologyTestId.Value, ct);
            if (test is null)
                return Result<RadiologyOrderDto>.Failure(
                    new(
                        "Radiology.TestNotFound",
                        "Radiology test was not found.",
                        ErrorType.NotFound
                    )
                );
        }
        var order = new RadiologyOrder(
            tenant.TenantId,
            command.PatientId,
            command.DoctorId,
            test?.Name ?? command.Procedure.Trim(),
            test?.Id
        );
        await repository.AddAsync(order, ct);
        await repository.SaveChangesAsync(ct);
        return Result<RadiologyOrderDto>.Success(RadiologyOrderDto.From(order));
    }
}
