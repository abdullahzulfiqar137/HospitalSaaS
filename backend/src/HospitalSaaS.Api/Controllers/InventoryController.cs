using HospitalSaaS.Api.Authorization;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Application.Inventory;
using HospitalSaaS.Contracts.Common;
using HospitalSaaS.Contracts.Inventory;
using HospitalSaaS.Domain.Inventory;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSaaS.Api.Controllers;

[Route("api/v1/inventory")]
public sealed class InventoryController(IRequestDispatcher dispatcher) : ApiControllerBase
{
    [HttpPost, HasPermission("Inventory.Create")]
    public async Task<IActionResult> Create(CreateInventoryItemRequest r, CancellationToken ct)
    {
        var x = await dispatcher.Send<CreateInventoryItemCommand, Result<InventoryItemDto>>(
            new(r.Sku, r.Name, r.Unit, r.MinimumStock),
            ct
        );
        return x.IsSuccess
            ? CreatedAtAction(nameof(Get), new { id = x.Value!.Id }, Map(x.Value))
            : FromError(x.Error!);
    }

    [HttpGet("{id:guid}"), HasPermission("Inventory.View")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var x = await dispatcher.Send<GetInventoryItemQuery, Result<InventoryItemDto>>(new(id), ct);
        return x.IsSuccess ? Ok(Map(x.Value!)) : FromError(x.Error!);
    }

    [HttpGet, HasPermission("Inventory.View")]
    public async Task<IActionResult> List(
        string? search,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken ct = default
    )
    {
        var x = await dispatcher.Send<SearchInventoryItemsQuery, PagedResult<InventoryItemDto>>(
            new(search, pageNumber, pageSize),
            ct
        );
        return Ok(
            new PagedResponse<InventoryItemResponse>(
                x.Items.Select(Map).ToList(),
                x.PageNumber,
                x.PageSize,
                x.TotalCount,
                x.TotalPages,
                x.HasPreviousPage,
                x.HasNextPage
            )
        );
    }

    [HttpDelete("{id:guid}"), HasPermission("Inventory.Update")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var x = await dispatcher.Send<DeleteInventoryItemCommand, Result>(new(id), ct);
        return x.IsSuccess ? NoContent() : FromError(x.Error!);
    }

    [HttpPost("{id:guid}/adjustments"), HasPermission("Inventory.AdjustStock")]
    public async Task<IActionResult> Adjust(Guid id, AdjustInventoryRequest r, CancellationToken ct)
    {
        var x = await dispatcher.Send<AdjustInventoryStockCommand, Result<InventoryTransactionDto>>(
            new(id, r.Quantity, r.Reason),
            ct
        );
        return x.IsSuccess
            ? Ok(
                new InventoryTransactionResponse(
                    x.Value!.Id,
                    x.Value.InventoryItemId,
                    x.Value.Quantity,
                    x.Value.Type,
                    x.Value.Reason,
                    x.Value.OccurredAtUtc
                )
            )
            : FromError(x.Error!);
    }

    [HttpGet("{id:guid}/stock"), HasPermission("Inventory.View")]
    public async Task<IActionResult> Stock(Guid id, CancellationToken ct)
    {
        var x = await dispatcher.Send<GetInventoryStockQuery, Result<int>>(new(id), ct);
        return x.IsSuccess ? Ok(new { quantity = x.Value }) : FromError(x.Error!);
    }

    [HttpPost("purchase-orders"), HasPermission("Inventory.Create")]
    public async Task<IActionResult> CreatePurchaseOrder(
        CreatePurchaseOrderRequest request,
        CancellationToken ct
    )
    {
        var result = await dispatcher.Send<CreatePurchaseOrderCommand, Result<PurchaseOrderDto>>(
            new(
                request.SupplierId,
                request.ExpectedDeliveryDate,
                request.Notes,
                request
                    .Items.Select(x => new PurchaseOrderItemInput(
                        x.InventoryItemId,
                        x.Quantity,
                        x.UnitCost
                    ))
                    .ToList()
            ),
            ct
        );
        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetPurchaseOrder),
                new { id = result.Value!.Id },
                Map(result.Value)
            )
            : FromError(result.Error!);
    }

    [HttpGet("purchase-orders/{id:guid}"), HasPermission("Inventory.View")]
    public async Task<IActionResult> GetPurchaseOrder(Guid id, CancellationToken ct)
    {
        var result = await dispatcher.Send<GetPurchaseOrderQuery, Result<PurchaseOrderDto>>(
            new(id),
            ct
        );
        return result.IsSuccess ? Ok(Map(result.Value!)) : FromError(result.Error!);
    }

    [HttpGet("purchase-orders"), HasPermission("Inventory.View")]
    public async Task<IActionResult> PurchaseOrders(
        PurchaseOrderStatus? status,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken ct = default
    )
    {
        var page = await dispatcher.Send<SearchPurchaseOrdersQuery, PagedResult<PurchaseOrderDto>>(
            new(status, pageNumber, pageSize),
            ct
        );
        return Ok(
            new PagedResponse<PurchaseOrderResponse>(
                page.Items.Select(Map).ToList(),
                page.PageNumber,
                page.PageSize,
                page.TotalCount,
                page.TotalPages,
                page.HasPreviousPage,
                page.HasNextPage
            )
        );
    }

    [HttpPost("purchase-orders/{id:guid}/actions"), HasPermission("Inventory.AdjustStock")]
    public async Task<IActionResult> PurchaseOrderAction(
        Guid id,
        PurchaseOrderActionRequest request,
        CancellationToken ct
    )
    {
        if (!Enum.TryParse<PurchaseOrderAction>(request.Action, true, out var action))
            return FromError(
                new("PurchaseOrders.Action", "Unknown purchase order action.", ErrorType.Validation)
            );
        var result = await dispatcher.Send<
            ChangePurchaseOrderStatusCommand,
            Result<PurchaseOrderDto>
        >(new(id, action), ct);
        return result.IsSuccess ? Ok(Map(result.Value!)) : FromError(result.Error!);
    }

    private static InventoryItemResponse Map(InventoryItemDto x) =>
        new(x.Id, x.Sku, x.Name, x.Unit, x.MinimumStock, x.IsActive);

    private static PurchaseOrderResponse Map(PurchaseOrderDto x) =>
        new(
            x.Id,
            x.OrderNumber,
            x.SupplierId,
            x.OrderedAtUtc,
            x.ExpectedDeliveryDate,
            x.Notes,
            x.Status.ToString(),
            x.ReceivedAtUtc,
            x.Total,
            x.Items.Select(item => new PurchaseOrderItemResponse(
                    item.Id,
                    item.InventoryItemId,
                    item.Quantity,
                    item.UnitCost,
                    item.Total
                ))
                .ToList()
        );
}
