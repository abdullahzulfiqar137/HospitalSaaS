using System.Security.Claims;
using HospitalSaaS.Api.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace HospitalSaaS.Api.Tests;

public sealed class PermissionHandlerTests
{
    [Fact]
    public async Task Exact_permission_claim_satisfies_requirement()
    {
        var requirement = new PermissionRequirement("Patients.View");
        var principal = new ClaimsPrincipal(
            new ClaimsIdentity([new Claim("permission", "Patients.View")], "test")
        );
        var context = new AuthorizationHandlerContext([requirement], principal, null);

        await new PermissionHandler().HandleAsync(context);

        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task Different_permission_does_not_satisfy_requirement()
    {
        var requirement = new PermissionRequirement("Patients.Delete");
        var principal = new ClaimsPrincipal(
            new ClaimsIdentity([new Claim("permission", "Patients.View")], "test")
        );
        var context = new AuthorizationHandlerContext([requirement], principal, null);

        await new PermissionHandler().HandleAsync(context);

        Assert.False(context.HasSucceeded);
    }
}
