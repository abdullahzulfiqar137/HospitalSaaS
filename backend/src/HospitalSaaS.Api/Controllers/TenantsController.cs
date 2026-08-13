using HospitalSaaS.Api.Authorization;
using HospitalSaaS.Application.Administration;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Contracts.Tenants;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSaaS.Api.Controllers;

[Route("api/v1/tenants")]
public sealed class TenantsController(IRequestDispatcher dispatcher) : ApiControllerBase
{
    [HttpPost, HasPermission("Tenants.Create")]
    public async Task<IActionResult> Create(
        CreateTenantRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await dispatcher.Send<CreateTenantCommand, Result<TenantDto>>(
            new CreateTenantCommand(request.Name, request.Slug),
            cancellationToken
        );

        return result.IsSuccess
            ? Ok(
                new TenantResponse(
                    result.Value!.Id,
                    result.Value.Name,
                    result.Value.Slug,
                    result.Value.IsActive
                )
            )
            : FromError(result.Error!);
    }
}
