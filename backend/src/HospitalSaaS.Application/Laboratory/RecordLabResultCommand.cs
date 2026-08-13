using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Domain.Laboratory;

namespace HospitalSaaS.Application.Laboratory;

public sealed record RecordLabResultCommand(
    Guid OrderId,
    Guid ItemId,
    string Result,
    string? Unit,
    string? ReferenceRange
) : ICommand<Result<LabOrderDto>>;

public sealed class RecordLabResultCommandValidator : AbstractValidator<RecordLabResultCommand>
{
    public RecordLabResultCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.Result).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.Unit).MaximumLength(100);
        RuleFor(x => x.ReferenceRange).MaximumLength(200);
    }
}

public sealed class RecordLabResultCommandHandler(ILaboratoryRepository repo, ITenantContext tenant)
    : IRequestHandler<RecordLabResultCommand, Result<LabOrderDto>>
{
    public async Task<Result<LabOrderDto>> Handle(RecordLabResultCommand x, CancellationToken ct)
    {
        var e = await repo.GetAsync(x.OrderId, true, ct);
        if (e is null)
            return Result<LabOrderDto>.Failure(
                new("Lab.NotFound", "Lab order was not found.", ErrorType.NotFound)
            );
        var item = e.Items.SingleOrDefault(i => i.Id == x.ItemId);
        if (item is null)
            return Result<LabOrderDto>.Failure(
                new("Lab.ItemNotFound", "Lab test was not found.", ErrorType.NotFound)
            );
        item.RecordResult(x.Result.Trim(), x.Unit, x.ReferenceRange);
        var version = await repo.CountResultsAsync(item.Id, ct) + 1;
        await repo.AddResultAsync(
            new LabResult(
                tenant.TenantId,
                e.Id,
                item.Id,
                x.Result,
                x.Unit,
                x.ReferenceRange,
                version
            ),
            ct
        );
        if (e.Items.All(i => i.Result is not null))
            e.Complete();
        await repo.SaveChangesAsync(ct);
        return Result<LabOrderDto>.Success(LabOrderDto.From(e));
    }
}
