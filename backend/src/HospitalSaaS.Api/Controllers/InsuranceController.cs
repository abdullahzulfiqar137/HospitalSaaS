using HospitalSaaS.Api.Authorization;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Application.Insurance;
using HospitalSaaS.Contracts.Common;
using HospitalSaaS.Contracts.Support;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSaaS.Api.Controllers;

[Route("api/v1/insurance")]
public sealed class InsuranceController(IRequestDispatcher dispatcher) : ApiControllerBase
{
    [HttpPost, HasPermission("Patients.Update")]
    public async Task<IActionResult> Create(CreateInsurancePolicyRequest r, CancellationToken ct)
    {
        var x = await dispatcher.Send<CreateInsurancePolicyCommand, Result<InsurancePolicyDto>>(
            new(r.PatientId, r.Provider, r.PolicyNumber, r.ValidFrom, r.ValidTo),
            ct
        );
        return x.IsSuccess ? Ok(Map(x.Value!)) : FromError(x.Error!);
    }

    [HttpGet, HasPermission("Patients.View")]
    public async Task<IActionResult> List(
        Guid? patientId,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken ct = default
    )
    {
        var x = await dispatcher.Send<
            SearchInsurancePoliciesQuery,
            PagedResult<InsurancePolicyDto>
        >(new(patientId, pageNumber, pageSize), ct);
        return Ok(
            new PagedResponse<InsurancePolicyResponse>(
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

    private static InsurancePolicyResponse Map(InsurancePolicyDto x) =>
        new(x.Id, x.PatientId, x.Provider, x.PolicyNumber, x.ValidFrom, x.ValidTo);
}
