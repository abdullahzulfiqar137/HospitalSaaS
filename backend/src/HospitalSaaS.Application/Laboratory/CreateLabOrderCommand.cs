using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Domain.Laboratory;

namespace HospitalSaaS.Application.Laboratory;

public sealed record LabTestInput(string Code, string Name, Guid? LabTestId = null);

public sealed record CreateLabOrderCommand(
    Guid PatientId,
    Guid DoctorId,
    IReadOnlyList<LabTestInput> Tests
) : ICommand<Result<LabOrderDto>>;

public sealed class CreateLabOrderCommandValidator : AbstractValidator<CreateLabOrderCommand>
{
    public CreateLabOrderCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.Tests).NotEmpty();
        RuleForEach(x => x.Tests)
            .ChildRules(t =>
            {
                t.RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
                t.RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            });
    }
}

public sealed class CreateLabOrderCommandHandler(ILaboratoryRepository repo, ITenantContext tenant)
    : IRequestHandler<CreateLabOrderCommand, Result<LabOrderDto>>
{
    public async Task<Result<LabOrderDto>> Handle(CreateLabOrderCommand x, CancellationToken ct)
    {
        if (
            x.Tests.Select(t => t.Code.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Count()
            != x.Tests.Count
        )
            return Result<LabOrderDto>.Failure(
                new(
                    "Lab.DuplicateTest",
                    "An order cannot contain duplicate test codes.",
                    ErrorType.Validation
                )
            );
        var catalogIds = x
            .Tests.Where(t => t.LabTestId.HasValue)
            .Select(t => t.LabTestId!.Value)
            .ToList();
        var catalog = await repo.GetTestsAsync(catalogIds, ct);
        if (catalog.Count != catalogIds.Distinct().Count())
            return Result<LabOrderDto>.Failure(
                new("Lab.TestNotFound", "One or more lab tests were not found.", ErrorType.NotFound)
            );

        var e = new LabOrder(tenant.TenantId, x.PatientId, x.DoctorId);
        foreach (var input in x.Tests)
        {
            var test = input.LabTestId.HasValue
                ? catalog.Single(t => t.Id == input.LabTestId.Value)
                : null;
            e.AddTest(
                test?.Code ?? input.Code.Trim().ToUpperInvariant(),
                test?.Name ?? input.Name.Trim(),
                test?.Id
            );
        }
        await repo.AddAsync(e, ct);
        await repo.SaveChangesAsync(ct);
        return Result<LabOrderDto>.Success(LabOrderDto.From(e));
    }
}
