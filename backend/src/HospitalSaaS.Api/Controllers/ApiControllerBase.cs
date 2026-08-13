using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Contracts.Common;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSaaS.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult FromError(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict or ErrorType.BusinessRule => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError,
        };

        return StatusCode(
            statusCode,
            new ErrorResponse(error.Code, error.Message, HttpContext.TraceIdentifier)
        );
    }
}
