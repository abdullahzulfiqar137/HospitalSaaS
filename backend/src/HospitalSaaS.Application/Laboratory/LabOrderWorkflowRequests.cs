using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;

namespace HospitalSaaS.Application.Laboratory;

public enum LabOrderAction
{
    CollectSample,
    StartProcessing,
    Cancel,
}

public sealed record ChangeLabOrderStatusCommand(Guid Id, LabOrderAction Action)
    : ICommand<Result<LabOrderDto>>;

public sealed class ChangeLabOrderStatusCommandValidator
    : AbstractValidator<ChangeLabOrderStatusCommand>
{
    public ChangeLabOrderStatusCommandValidator() => RuleFor(x => x.Id).NotEmpty();
}

public sealed class ChangeLabOrderStatusCommandHandler(ILaboratoryRepository repository)
    : IRequestHandler<ChangeLabOrderStatusCommand, Result<LabOrderDto>>
{
    public async Task<Result<LabOrderDto>> Handle(
        ChangeLabOrderStatusCommand command,
        CancellationToken cancellationToken
    )
    {
        var order = await repository.GetAsync(command.Id, true, cancellationToken);
        if (order is null)
            return Result<LabOrderDto>.Failure(
                new("Lab.NotFound", "Lab order was not found.", ErrorType.NotFound)
            );
        switch (command.Action)
        {
            case LabOrderAction.CollectSample:
                order.MarkSampleCollected();
                break;
            case LabOrderAction.StartProcessing:
                order.StartProcessing();
                break;
            case LabOrderAction.Cancel:
                order.Cancel();
                break;
        }
        await repository.SaveChangesAsync(cancellationToken);
        return Result<LabOrderDto>.Success(LabOrderDto.From(order));
    }
}

public sealed record GetLabResultHistoryQuery(Guid OrderId) : IQuery<IReadOnlyList<LabResultDto>>;

public sealed class GetLabResultHistoryQueryHandler(ILaboratoryRepository repository)
    : IRequestHandler<GetLabResultHistoryQuery, IReadOnlyList<LabResultDto>>
{
    public async Task<IReadOnlyList<LabResultDto>> Handle(
        GetLabResultHistoryQuery query,
        CancellationToken cancellationToken
    ) =>
        (await repository.GetResultsAsync(query.OrderId, cancellationToken))
            .Select(LabResultDto.From)
            .ToList();
}
