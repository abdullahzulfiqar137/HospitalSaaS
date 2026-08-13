using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Domain.Radiology;

namespace HospitalSaaS.Application.Radiology;

public sealed record RecordRadiologyReportCommand(Guid Id, string Report)
    : ICommand<Result<RadiologyOrderDto>>;

public sealed class RecordRadiologyReportCommandValidator
    : AbstractValidator<RecordRadiologyReportCommand>
{
    public RecordRadiologyReportCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Report).NotEmpty().MaximumLength(16000);
    }
}

public sealed class RecordRadiologyReportCommandHandler(
    IRadiologyRepository repository,
    ITenantContext tenant
) : IRequestHandler<RecordRadiologyReportCommand, Result<RadiologyOrderDto>>
{
    public async Task<Result<RadiologyOrderDto>> Handle(
        RecordRadiologyReportCommand command,
        CancellationToken ct
    )
    {
        var order = await repository.GetAsync(command.Id, true, ct);
        if (order is null)
            return Result<RadiologyOrderDto>.Failure(
                new("Radiology.NotFound", "Radiology order was not found.", ErrorType.NotFound)
            );
        order.RecordReport(command.Report.Trim());
        var version = await repository.CountResultsAsync(order.Id, ct) + 1;
        await repository.AddResultAsync(
            new RadiologyResult(tenant.TenantId, order.Id, command.Report, version),
            ct
        );
        await repository.SaveChangesAsync(ct);
        return Result<RadiologyOrderDto>.Success(RadiologyOrderDto.From(order));
    }
}
