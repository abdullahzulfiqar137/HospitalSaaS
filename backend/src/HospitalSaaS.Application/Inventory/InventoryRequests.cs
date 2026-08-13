using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Inventory;

namespace HospitalSaaS.Application.Inventory;

public sealed record CreateInventoryItemCommand(
    string Sku,
    string Name,
    string Unit,
    int MinimumStock
) : ICommand<Result<InventoryItemDto>>;

public sealed class CreateInventoryItemCommandValidator
    : AbstractValidator<CreateInventoryItemCommand>
{
    public CreateInventoryItemCommandValidator()
    {
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Unit).NotEmpty().MaximumLength(50);
        RuleFor(x => x.MinimumStock).GreaterThanOrEqualTo(0);
    }
}

public sealed class CreateInventoryItemCommandHandler(IInventoryRepository r, ITenantContext t)
    : IRequestHandler<CreateInventoryItemCommand, Result<InventoryItemDto>>
{
    public async Task<Result<InventoryItemDto>> Handle(
        CreateInventoryItemCommand x,
        CancellationToken ct
    )
    {
        if (await r.SkuExistsAsync(x.Sku, ct))
            return Result<InventoryItemDto>.Failure(
                new("Inventory.SkuConflict", "SKU already exists.", ErrorType.Conflict)
            );
        var e = new InventoryItem(
            t.TenantId,
            x.Sku.Trim().ToUpperInvariant(),
            x.Name.Trim(),
            x.Unit.Trim(),
            x.MinimumStock
        );
        await r.AddItemAsync(e, ct);
        await r.SaveChangesAsync(ct);
        return Result<InventoryItemDto>.Success(InventoryItemDto.From(e));
    }
}

public sealed record GetInventoryItemQuery(Guid Id) : IQuery<Result<InventoryItemDto>>;

public sealed class GetInventoryItemQueryHandler(IInventoryRepository r)
    : IRequestHandler<GetInventoryItemQuery, Result<InventoryItemDto>>
{
    public async Task<Result<InventoryItemDto>> Handle(
        GetInventoryItemQuery x,
        CancellationToken ct
    )
    {
        var e = await r.GetItemAsync(x.Id, false, ct);
        return e is null
            ? Result<InventoryItemDto>.Failure(
                new("Inventory.NotFound", "Inventory item was not found.", ErrorType.NotFound)
            )
            : Result<InventoryItemDto>.Success(InventoryItemDto.From(e));
    }
}

public sealed record SearchInventoryItemsQuery(string? Search, int PageNumber, int PageSize)
    : IQuery<PagedResult<InventoryItemDto>>;

public sealed class SearchInventoryItemsQueryValidator
    : AbstractValidator<SearchInventoryItemsQuery>
{
    public SearchInventoryItemsQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class SearchInventoryItemsQueryHandler(IInventoryRepository r)
    : IRequestHandler<SearchInventoryItemsQuery, PagedResult<InventoryItemDto>>
{
    public async Task<PagedResult<InventoryItemDto>> Handle(
        SearchInventoryItemsQuery x,
        CancellationToken ct
    )
    {
        var p = await r.SearchAsync(x.Search, x.PageNumber, x.PageSize, ct);
        return new(
            p.Items.Select(InventoryItemDto.From).ToList(),
            p.PageNumber,
            p.PageSize,
            p.TotalCount
        );
    }
}

public sealed record DeleteInventoryItemCommand(Guid Id) : ICommand<Result>;

public sealed class DeleteInventoryItemCommandHandler(IInventoryRepository r)
    : IRequestHandler<DeleteInventoryItemCommand, Result>
{
    public async Task<Result> Handle(DeleteInventoryItemCommand x, CancellationToken ct)
    {
        var e = await r.GetItemAsync(x.Id, true, ct);
        if (e is null)
            return Result.Failure(
                new("Inventory.NotFound", "Inventory item was not found.", ErrorType.NotFound)
            );
        r.RemoveItem(e);
        await r.SaveChangesAsync(ct);
        return Result.Success();
    }
}
