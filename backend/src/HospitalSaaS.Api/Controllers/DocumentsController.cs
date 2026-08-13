using HospitalSaaS.Api.Authorization;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Application.Documents;
using HospitalSaaS.Contracts.Common;
using HospitalSaaS.Contracts.Support;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSaaS.Api.Controllers;

[Route("api/v1/documents")]
public sealed class DocumentsController(IFileStorageService storage, IRequestDispatcher dispatcher)
    : ApiControllerBase
{
    [HttpPost, RequestSizeLimit(20_000_000), HasPermission("Documents.Create")]
    public async Task<IActionResult> Upload(
        [FromForm] Guid patientId,
        IFormFile file,
        CancellationToken ct
    )
    {
        if (file.Length <= 0 || file.Length > 20_000_000)
            return FromError(
                new(
                    "Documents.Size",
                    "File size must be between 1 byte and 20 MB.",
                    ErrorType.Validation
                )
            );
        await using var stream = file.OpenReadStream();
        var key = await storage.SaveAsync(stream, file.FileName, file.ContentType, ct);
        var x = await dispatcher.Send<CreateDocumentCommand, Result<DocumentDto>>(
            new(patientId, file.FileName, file.ContentType, file.Length, key),
            ct
        );
        return x.IsSuccess ? Ok(Map(x.Value!)) : FromError(x.Error!);
    }

    [HttpGet, HasPermission("Documents.View")]
    public async Task<IActionResult> List(
        Guid? patientId,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken ct = default
    )
    {
        var x = await dispatcher.Send<SearchDocumentsQuery, PagedResult<DocumentDto>>(
            new(patientId, pageNumber, pageSize),
            ct
        );
        return Ok(
            new PagedResponse<DocumentResponse>(
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

    [HttpDelete("{id:guid}"), HasPermission("Documents.Create")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var x = await dispatcher.Send<DeleteDocumentCommand, Result>(new(id), ct);
        return x.IsSuccess ? NoContent() : FromError(x.Error!);
    }

    private static DocumentResponse Map(DocumentDto x) =>
        new(x.Id, x.PatientId, x.Name, x.ContentType, x.Size);
}
