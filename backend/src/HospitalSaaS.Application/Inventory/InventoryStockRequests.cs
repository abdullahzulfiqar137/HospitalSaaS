using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Domain.Inventory;

namespace HospitalSaaS.Application.Inventory;

public sealed record AdjustInventoryStockCommand(Guid ItemId, int Quantity, string Reason)
    : ICommand<Result<InventoryTransactionDto>>;

public sealed class AdjustInventoryStockCommandValidator
    : AbstractValidator<AdjustInventoryStockCommand>
{
    public AdjustInventoryStockCommandValidator()
    {
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.Quantity).NotEqual(0);
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}

public sealed class AdjustInventoryStockCommandHandler(IInventoryRepository r, ITenantContext t)
    : IRequestHandler<AdjustInventoryStockCommand, Result<InventoryTransactionDto>>
{
    public async Task<Result<InventoryTransactionDto>> Handle(
        AdjustInventoryStockCommand x,
        CancellationToken ct
    )
    {
        if (await r.GetItemAsync(x.ItemId, false, ct) is null)
            return Result<InventoryTransactionDto>.Failure(
                new("Inventory.NotFound", "Inventory item was not found.", ErrorType.NotFound)
            );
        if (await r.GetBalanceAsync(x.ItemId, ct) + x.Quantity < 0)
            return Result<InventoryTransactionDto>.Failure(
                new(
                    "Inventory.Insufficient",
                    "Inventory stock cannot become negative.",
                    ErrorType.Conflict
                )
            );
        var e = new InventoryTransaction(
            t.TenantId,
            x.ItemId,
            x.Quantity,
            "Adjustment",
            x.Reason.Trim()
        );
        await r.AddTransactionAsync(e, ct);
        await r.SaveChangesAsync(ct);
        return Result<InventoryTransactionDto>.Success(InventoryTransactionDto.From(e));
    }
}

public sealed record GetInventoryStockQuery(Guid ItemId) : IQuery<Result<int>>;

public sealed class GetInventoryStockQueryHandler(IInventoryRepository r)
    : IRequestHandler<GetInventoryStockQuery, Result<int>>
{
    public async Task<Result<int>> Handle(GetInventoryStockQuery x, CancellationToken ct)
    {
        if (await r.GetItemAsync(x.ItemId, false, ct) is null)
            return Result<int>.Failure(
                new("Inventory.NotFound", "Inventory item was not found.", ErrorType.NotFound)
            );
        return Result<int>.Success(await r.GetBalanceAsync(x.ItemId, ct));
    }
}
