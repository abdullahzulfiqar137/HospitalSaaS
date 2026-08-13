using HospitalSaaS.Api.Authorization;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Application.Pharmacy;
using HospitalSaaS.Contracts.Common;
using HospitalSaaS.Contracts.Pharmacy;
using HospitalSaaS.Domain.Pharmacy;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSaaS.Api.Controllers;

[Route("api/v1/pharmacy")]
public sealed class PharmacyController(IRequestDispatcher dispatcher) : ApiControllerBase
{
    [HttpPost("medicines"), HasPermission("Pharmacy.Create")]
    public async Task<IActionResult> Create(CreateMedicineRequest r, CancellationToken ct)
    {
        var c = new CreateMedicineCommand(
            r.Name,
            r.GenericName,
            r.BrandName,
            r.Strength,
            r.DosageForm,
            r.Manufacturer,
            r.Barcode,
            r.SalePrice,
            r.PurchasePrice,
            r.MinimumStockLevel
        );
        var x = await dispatcher.Send<CreateMedicineCommand, Result<MedicineDto>>(c, ct);
        return x.IsSuccess
            ? CreatedAtAction(nameof(Get), new { id = x.Value!.Id }, Map(x.Value))
            : FromError(x.Error!);
    }

    [HttpGet("medicines/{id:guid}"), HasPermission("Pharmacy.View")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var x = await dispatcher.Send<GetMedicineQuery, Result<MedicineDto>>(new(id), ct);
        return x.IsSuccess ? Ok(Map(x.Value!)) : FromError(x.Error!);
    }

    [HttpGet("medicines"), HasPermission("Pharmacy.View")]
    public async Task<IActionResult> List(
        string? search,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken ct = default
    )
    {
        var x = await dispatcher.Send<SearchMedicinesQuery, PagedResult<MedicineDto>>(
            new(search, pageNumber, pageSize),
            ct
        );
        return Ok(
            new PagedResponse<MedicineResponse>(
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

    [HttpDelete("medicines/{id:guid}"), HasPermission("Pharmacy.Create")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var x = await dispatcher.Send<DeleteMedicineCommand, Result>(new(id), ct);
        return x.IsSuccess ? NoContent() : FromError(x.Error!);
    }

    [HttpPost("medicines/{id:guid}/stock"), HasPermission("Pharmacy.Dispense")]
    public async Task<IActionResult> Stock(Guid id, StockRequest r, CancellationToken ct)
    {
        if (!Enum.TryParse<StockTransactionType>(r.Type, true, out var type))
            return FromError(
                new("Stock.Type", "Unknown stock transaction type.", ErrorType.Validation)
            );
        var x = await dispatcher.Send<RecordMedicineStockCommand, Result<StockTransactionDto>>(
            new(id, type, r.Quantity, r.Reference),
            ct
        );
        return x.IsSuccess
            ? Ok(
                new StockTransactionResponse(
                    x.Value!.Id,
                    x.Value.MedicineId,
                    x.Value.Type.ToString(),
                    x.Value.Quantity,
                    x.Value.Reference,
                    x.Value.OccurredAtUtc
                )
            )
            : FromError(x.Error!);
    }

    [HttpGet("medicines/{id:guid}/stock"), HasPermission("Pharmacy.View")]
    public async Task<IActionResult> Balance(Guid id, CancellationToken ct)
    {
        var x = await dispatcher.Send<GetMedicineStockQuery, Result<int>>(new(id), ct);
        return x.IsSuccess ? Ok(new { quantity = x.Value }) : FromError(x.Error!);
    }

    private static MedicineResponse Map(MedicineDto x) =>
        new(
            x.Id,
            x.Name,
            x.GenericName,
            x.BrandName,
            x.Strength,
            x.DosageForm,
            x.Manufacturer,
            x.Barcode,
            x.SalePrice,
            x.PurchasePrice,
            x.MinimumStockLevel,
            x.IsActive
        );
}
