using HospitalSaaS.Api.Authorization;
using HospitalSaaS.Application.Administration;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Contracts.Common;
using HospitalSaaS.Contracts.Users;
using Microsoft.AspNetCore.Mvc;

namespace HospitalSaaS.Api.Controllers;

[Route("api/v1/users")]
public sealed class UsersController(IRequestDispatcher dispatcher) : ApiControllerBase
{
    [HttpPost, HasPermission("Users.Create")]
    public async Task<IActionResult> Create(
        CreateUserRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new CreateUserCommand(request.Email, request.Password, request.RoleIds);
        var result = await dispatcher.Send<CreateUserCommand, Result<UserDto>>(
            command,
            cancellationToken
        );
        return result.IsSuccess ? Ok(Map(result.Value!)) : FromError(result.Error!);
    }

    [HttpGet, HasPermission("Users.View")]
    public async Task<IActionResult> List(
        string? search,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default
    )
    {
        var result = await dispatcher.Send<SearchUsersQuery, PagedResult<UserDto>>(
            new SearchUsersQuery(search, pageNumber, pageSize),
            cancellationToken
        );
        return Ok(
            new PagedResponse<UserResponse>(
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

    private static UserResponse Map(UserDto user) =>
        new(user.Id, user.Email, user.IsActive, user.Roles);
}
