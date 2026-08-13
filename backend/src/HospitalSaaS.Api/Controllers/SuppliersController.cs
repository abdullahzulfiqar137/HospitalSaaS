using HospitalSaaS.Api.Authorization;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Application.Suppliers;
using HospitalSaaS.Contracts.Common;
using HospitalSaaS.Contracts.Support;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSaaS.Api.Controllers;

[Route("api/v1/suppliers")]
public sealed class SuppliersController(IRequestDispatcher dispatcher) : ApiControllerBase
{
    [HttpPost, HasPermission("Inventory.Create")]
    public async Task<IActionResult> Create(CreateSupplierRequest r, CancellationToken ct)
    {
        var x = await dispatcher.Send<CreateSupplierCommand, Result<SupplierDto>>(
            new(r.Name, r.ContactName, r.Phone, r.Email, r.Address),
            ct
        );
        return x.IsSuccess
            ? CreatedAtAction(nameof(Get), new { id = x.Value!.Id }, Map(x.Value))
            : FromError(x.Error!);
    }

    [HttpGet("{id:guid}"), HasPermission("Inventory.View")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var x = await dispatcher.Send<GetSupplierQuery, Result<SupplierDto>>(new(id), ct);
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
        var x = await dispatcher.Send<SearchSuppliersQuery, PagedResult<SupplierDto>>(
            new(search, pageNumber, pageSize),
            ct
        );
        return Ok(
            new PagedResponse<SupplierResponse>(
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
        var x = await dispatcher.Send<DeleteSupplierCommand, Result>(new(id), ct);
        return x.IsSuccess ? NoContent() : FromError(x.Error!);
    }

    private static SupplierResponse Map(SupplierDto x) =>
        new(x.Id, x.Name, x.ContactName, x.Phone, x.Email, x.Address, x.IsActive);
}
