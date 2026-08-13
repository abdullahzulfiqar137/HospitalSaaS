using HospitalSaaS.Api.Authorization;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Application.Radiology;
using HospitalSaaS.Contracts.Clinical;
using HospitalSaaS.Contracts.Common;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSaaS.Api.Controllers;

[Route("api/v1/radiology")]
public sealed class RadiologyController(IRequestDispatcher dispatcher) : ApiControllerBase
{
    [HttpPost("tests"), HasPermission("Radiology.CreateOrder")]
    public async Task<IActionResult> CreateTest(
        CreateRadiologyTestRequest request,
        CancellationToken ct
    )
    {
        var result = await dispatcher.Send<CreateRadiologyTestCommand, Result<RadiologyTestDto>>(
            new(request.Code, request.Name, request.Modality, request.Price),
            ct
        );
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetTest), new { id = result.Value!.Id }, Map(result.Value))
            : FromError(result.Error!);
    }

    [HttpGet("tests/{id:guid}"), HasPermission("Radiology.View")]
    public async Task<IActionResult> GetTest(Guid id, CancellationToken ct)
    {
        var result = await dispatcher.Send<GetRadiologyTestQuery, Result<RadiologyTestDto>>(
            new(id),
            ct
        );
        return result.IsSuccess ? Ok(Map(result.Value!)) : FromError(result.Error!);
    }

    [HttpGet("tests"), HasPermission("Radiology.View")]
    public async Task<IActionResult> Tests(
        string? search,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken ct = default
    )
    {
        var page = await dispatcher.Send<SearchRadiologyTestsQuery, PagedResult<RadiologyTestDto>>(
            new(search, pageNumber, pageSize),
            ct
        );
        return Ok(
            new PagedResponse<RadiologyTestResponse>(
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

    [HttpPost("orders"), HasPermission("Radiology.CreateOrder")]
    public async Task<IActionResult> Create(
        CreateRadiologyOrderRequest request,
        CancellationToken ct
    )
    {
        var command = new CreateRadiologyOrderCommand(
            request.PatientId,
            request.DoctorId,
            request.Procedure,
            request.RadiologyTestId
        );
        var result = await dispatcher.Send<CreateRadiologyOrderCommand, Result<RadiologyOrderDto>>(
            command,
            ct
        );
        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, Map(result.Value))
            : FromError(result.Error!);
    }

    [HttpGet("orders/{id:guid}/results/history"), HasPermission("Radiology.View")]
    public async Task<IActionResult> ResultHistory(Guid id, CancellationToken ct)
    {
        var results = await dispatcher.Send<
            GetRadiologyResultHistoryQuery,
            IReadOnlyList<RadiologyResultDto>
        >(new(id), ct);
        return Ok(
            results.Select(x => new RadiologyResultResponse(
                x.Id,
                x.Report,
                x.Version,
                x.RecordedAtUtc
            ))
        );
    }

    [HttpGet("orders/{id:guid}"), HasPermission("Radiology.View")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var result = await dispatcher.Send<GetRadiologyOrderQuery, Result<RadiologyOrderDto>>(
            new(id),
            ct
        );
        return result.IsSuccess ? Ok(Map(result.Value!)) : FromError(result.Error!);
    }

    [HttpGet("orders"), HasPermission("Radiology.View")]
    public async Task<IActionResult> List(
        Guid? patientId,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken ct = default
    )
    {
        var result = await dispatcher.Send<
            SearchRadiologyOrdersQuery,
            PagedResult<RadiologyOrderDto>
        >(new(patientId, pageNumber, pageSize), ct);
        return Ok(
            new PagedResponse<RadiologyOrderResponse>(
                result.Items.Select(Map).ToList(),
                result.PageNumber,
                result.PageSize,
                result.TotalCount,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            )
        );
    }

    [HttpPost("orders/{id:guid}/report"), HasPermission("Radiology.RecordResult")]
    public async Task<IActionResult> Report(
        Guid id,
        RecordRadiologyReportRequest request,
        CancellationToken ct
    )
    {
        var result = await dispatcher.Send<RecordRadiologyReportCommand, Result<RadiologyOrderDto>>(
            new(id, request.Report),
            ct
        );
        return result.IsSuccess ? Ok(Map(result.Value!)) : FromError(result.Error!);
    }

    private static RadiologyOrderResponse Map(RadiologyOrderDto x) =>
        new(
            x.Id,
            x.PatientId,
            x.DoctorId,
            x.RadiologyTestId,
            x.Procedure,
            x.OrderedAtUtc,
            x.Status.ToString(),
            x.Report
        );

    private static RadiologyTestResponse Map(RadiologyTestDto x) =>
        new(x.Id, x.Code, x.Name, x.Modality, x.Price, x.IsActive);
}
