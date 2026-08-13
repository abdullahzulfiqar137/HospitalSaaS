using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Inventory;

namespace HospitalSaaS.Application.Inventory;

public sealed record PurchaseOrderItemInput(Guid InventoryItemId, int Quantity, decimal UnitCost);

public sealed record CreatePurchaseOrderCommand(
    Guid SupplierId,
    DateOnly? ExpectedDeliveryDate,
    string? Notes,
    IReadOnlyList<PurchaseOrderItemInput> Items
) : ICommand<Result<PurchaseOrderDto>>;

public sealed class CreatePurchaseOrderCommandValidator
    : AbstractValidator<CreatePurchaseOrderCommand>
{
    public CreatePurchaseOrderCommandValidator()
    {
        RuleFor(x => x.SupplierId).NotEmpty();
        RuleFor(x => x.ExpectedDeliveryDate)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .When(x => x.ExpectedDeliveryDate.HasValue);
        RuleFor(x => x.Notes).MaximumLength(2000);
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.InventoryItemId).NotEmpty();
                item.RuleFor(x => x.Quantity).GreaterThan(0);
                item.RuleFor(x => x.UnitCost).GreaterThanOrEqualTo(0);
            });
    }
}

public sealed class CreatePurchaseOrderCommandHandler(
    IInventoryRepository inventory,
    ISupplierRepository suppliers,
    ITenantContext tenant
) : IRequestHandler<CreatePurchaseOrderCommand, Result<PurchaseOrderDto>>
{
    public async Task<Result<PurchaseOrderDto>> Handle(
        CreatePurchaseOrderCommand command,
        CancellationToken cancellationToken
    )
    {
        if (await suppliers.GetAsync(command.SupplierId, false, cancellationToken) is null)
            return Result<PurchaseOrderDto>.Failure(
                new("Suppliers.NotFound", "Supplier was not found.", ErrorType.NotFound)
            );
        if (command.Items.Select(x => x.InventoryItemId).Distinct().Count() != command.Items.Count)
            return Result<PurchaseOrderDto>.Failure(
                new(
                    "PurchaseOrders.DuplicateItem",
                    "An item can appear only once in a purchase order.",
                    ErrorType.Validation
                )
            );
        foreach (var item in command.Items)
            if (
                await inventory.GetItemAsync(item.InventoryItemId, false, cancellationToken) is null
            )
                return Result<PurchaseOrderDto>.Failure(
                    new(
                        "Inventory.NotFound",
                        $"Inventory item {item.InventoryItemId} was not found.",
                        ErrorType.NotFound
                    )
                );

        var year = DateTime.UtcNow.Year;
        var sequence = await inventory.CountPurchaseOrdersInYearAsync(year, cancellationToken) + 1;
        var order = new PurchaseOrder(
            tenant.TenantId,
            $"PO-{year}-{sequence:000000}",
            command.SupplierId,
            command.ExpectedDeliveryDate,
            command.Notes
        );
        foreach (var item in command.Items)
            order.AddItem(item.InventoryItemId, item.Quantity, item.UnitCost);
        order.Submit();
        await inventory.AddPurchaseOrderAsync(order, cancellationToken);
        await inventory.SaveChangesAsync(cancellationToken);
        return Result<PurchaseOrderDto>.Success(PurchaseOrderDto.From(order));
    }
}

public sealed record GetPurchaseOrderQuery(Guid Id) : IQuery<Result<PurchaseOrderDto>>;

public sealed class GetPurchaseOrderQueryHandler(IInventoryRepository repository)
    : IRequestHandler<GetPurchaseOrderQuery, Result<PurchaseOrderDto>>
{
    public async Task<Result<PurchaseOrderDto>> Handle(
        GetPurchaseOrderQuery query,
        CancellationToken cancellationToken
    )
    {
        var order = await repository.GetPurchaseOrderAsync(query.Id, false, cancellationToken);
        return order is null
            ? Result<PurchaseOrderDto>.Failure(
                new("PurchaseOrders.NotFound", "Purchase order was not found.", ErrorType.NotFound)
            )
            : Result<PurchaseOrderDto>.Success(PurchaseOrderDto.From(order));
    }
}

public sealed record SearchPurchaseOrdersQuery(
    PurchaseOrderStatus? Status,
    int PageNumber,
    int PageSize
) : IQuery<PagedResult<PurchaseOrderDto>>;

public sealed class SearchPurchaseOrdersQueryValidator
    : AbstractValidator<SearchPurchaseOrdersQuery>
{
    public SearchPurchaseOrdersQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class SearchPurchaseOrdersQueryHandler(IInventoryRepository repository)
    : IRequestHandler<SearchPurchaseOrdersQuery, PagedResult<PurchaseOrderDto>>
{
    public async Task<PagedResult<PurchaseOrderDto>> Handle(
        SearchPurchaseOrdersQuery query,
        CancellationToken cancellationToken
    )
    {
        var page = await repository.SearchPurchaseOrdersAsync(
            query.Status,
            query.PageNumber,
            query.PageSize,
            cancellationToken
        );
        return new(
            page.Items.Select(PurchaseOrderDto.From).ToList(),
            page.PageNumber,
            page.PageSize,
            page.TotalCount
        );
    }
}

public enum PurchaseOrderAction
{
    Receive,
    Cancel,
}

public sealed record ChangePurchaseOrderStatusCommand(Guid Id, PurchaseOrderAction Action)
    : ICommand<Result<PurchaseOrderDto>>;

public sealed class ChangePurchaseOrderStatusCommandValidator
    : AbstractValidator<ChangePurchaseOrderStatusCommand>
{
    public ChangePurchaseOrderStatusCommandValidator() => RuleFor(x => x.Id).NotEmpty();
}

public sealed class ChangePurchaseOrderStatusCommandHandler(
    IInventoryRepository repository,
    ITenantContext tenant
) : IRequestHandler<ChangePurchaseOrderStatusCommand, Result<PurchaseOrderDto>>
{
    public async Task<Result<PurchaseOrderDto>> Handle(
        ChangePurchaseOrderStatusCommand command,
        CancellationToken cancellationToken
    )
    {
        var order = await repository.GetPurchaseOrderAsync(command.Id, true, cancellationToken);
        if (order is null)
            return Result<PurchaseOrderDto>.Failure(
                new("PurchaseOrders.NotFound", "Purchase order was not found.", ErrorType.NotFound)
            );
        if (command.Action == PurchaseOrderAction.Receive)
        {
            order.Receive();
            foreach (var item in order.Items)
                await repository.AddTransactionAsync(
                    new InventoryTransaction(
                        tenant.TenantId,
                        item.InventoryItemId,
                        item.Quantity,
                        "Purchase",
                        $"Received {order.OrderNumber}"
                    ),
                    cancellationToken
                );
        }
        else
        {
            order.Cancel();
        }
        await repository.SaveChangesAsync(cancellationToken);
        return Result<PurchaseOrderDto>.Success(PurchaseOrderDto.From(order));
    }
}
