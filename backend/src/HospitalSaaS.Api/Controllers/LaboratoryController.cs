using HospitalSaaS.Api.Authorization;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Application.Laboratory;
using HospitalSaaS.Contracts.Common;
using HospitalSaaS.Contracts.Laboratory;
using HospitalSaaS.Domain.Laboratory;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSaaS.Api.Controllers;

[Route("api/v1/laboratory")]
public sealed class LaboratoryController(IRequestDispatcher dispatcher) : ApiControllerBase
{
    [HttpPost("tests"), HasPermission("Laboratory.CreateOrder")]
    public async Task<IActionResult> CreateTest(CreateLabTestRequest request, CancellationToken ct)
    {
        var result = await dispatcher.Send<CreateLabTestCommand, Result<LabTestDto>>(
            new(
                request.Name,
                request.Code,
                request.Category,
                request.Price,
                request.NormalRange,
                request.Unit
            ),
            ct
        );
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetTest), new { id = result.Value!.Id }, Map(result.Value))
            : FromError(result.Error!);
    }

    [HttpGet("tests/{id:guid}"), HasPermission("Laboratory.View")]
    public async Task<IActionResult> GetTest(Guid id, CancellationToken ct)
    {
        var result = await dispatcher.Send<GetLabTestQuery, Result<LabTestDto>>(new(id), ct);
        return result.IsSuccess ? Ok(Map(result.Value!)) : FromError(result.Error!);
    }

    [HttpGet("tests"), HasPermission("Laboratory.View")]
    public async Task<IActionResult> Tests(
        string? search,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken ct = default
    )
    {
        var page = await dispatcher.Send<SearchLabTestsQuery, PagedResult<LabTestDto>>(
            new(search, pageNumber, pageSize),
            ct
        );
        return Ok(
            new PagedResponse<LabTestResponse>(
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

    [HttpPost("orders"), HasPermission("Laboratory.CreateOrder")]
    public async Task<IActionResult> Create(CreateLabOrderRequest r, CancellationToken ct)
    {
        var c = new CreateLabOrderCommand(
            r.PatientId,
            r.DoctorId,
            r.Tests.Select(t => new LabTestInput(t.Code, t.Name, t.LabTestId)).ToList()
        );
        var x = await dispatcher.Send<CreateLabOrderCommand, Result<LabOrderDto>>(c, ct);
        return x.IsSuccess
            ? CreatedAtAction(nameof(Get), new { id = x.Value!.Id }, Map(x.Value))
            : FromError(x.Error!);
    }

    [HttpPost("orders/{id:guid}/actions"), HasPermission("Laboratory.RecordResult")]
    public async Task<IActionResult> Action(
        Guid id,
        LabOrderActionRequest request,
        CancellationToken ct
    )
    {
        if (!Enum.TryParse<LabOrderAction>(request.Action, true, out var action))
            return FromError(new("Lab.Action", "Unknown lab order action.", ErrorType.Validation));
        var result = await dispatcher.Send<ChangeLabOrderStatusCommand, Result<LabOrderDto>>(
            new(id, action),
            ct
        );
        return result.IsSuccess ? Ok(Map(result.Value!)) : FromError(result.Error!);
    }

    [HttpGet("orders/{id:guid}/results/history"), HasPermission("Laboratory.View")]
    public async Task<IActionResult> ResultHistory(Guid id, CancellationToken ct)
    {
        var results = await dispatcher.Send<GetLabResultHistoryQuery, IReadOnlyList<LabResultDto>>(
            new(id),
            ct
        );
        return Ok(
            results.Select(x => new LabResultResponse(
                x.Id,
                x.LabOrderItemId,
                x.Value,
                x.Unit,
                x.ReferenceRange,
                x.Version,
                x.RecordedAtUtc
            ))
        );
    }

    [HttpGet("orders/{id:guid}"), HasPermission("Laboratory.View")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var x = await dispatcher.Send<GetLabOrderQuery, Result<LabOrderDto>>(new(id), ct);
        return x.IsSuccess ? Ok(Map(x.Value!)) : FromError(x.Error!);
    }

    [HttpGet("orders"), HasPermission("Laboratory.View")]
    public async Task<IActionResult> List(
        Guid? patientId,
        LabOrderStatus? status,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken ct = default
    )
    {
        var x = await dispatcher.Send<SearchLabOrdersQuery, PagedResult<LabOrderDto>>(
            new(patientId, status, pageNumber, pageSize),
            ct
        );
        return Ok(
            new PagedResponse<LabOrderResponse>(
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

    [HttpPost("orders/{id:guid}/results"), HasPermission("Laboratory.RecordResult")]
    public async Task<IActionResult> Result(Guid id, RecordLabResultRequest r, CancellationToken ct)
    {
        var x = await dispatcher.Send<RecordLabResultCommand, Result<LabOrderDto>>(
            new(id, r.ItemId, r.Result, r.Unit, r.ReferenceRange),
            ct
        );
        return x.IsSuccess ? Ok(Map(x.Value!)) : FromError(x.Error!);
    }

    private static LabOrderResponse Map(LabOrderDto x) =>
        new(
            x.Id,
            x.PatientId,
            x.DoctorId,
            x.OrderedAtUtc,
            x.Status.ToString(),
            x.Items.Select(i => new LabOrderItemResponse(
                    i.Id,
                    i.LabTestId,
                    i.TestCode,
                    i.TestName,
                    i.Result,
                    i.Unit,
                    i.ReferenceRange
                ))
                .ToList()
        );

    private static LabTestResponse Map(LabTestDto x) =>
        new(x.Id, x.Name, x.Code, x.Category, x.Price, x.NormalRange, x.Unit, x.IsActive);
}
