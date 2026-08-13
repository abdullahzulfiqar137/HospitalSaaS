using System.Security.Claims;
using HospitalSaaS.Application.Common.Interfaces;

namespace HospitalSaaS.Api.Middleware;

public sealed class TenantResolutionMiddleware(RequestDelegate next)
{
    public const string TenantHeaderName = "X-Tenant-ID";

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        var rawTenantId =
            context.User.Identity?.IsAuthenticated == true
                ? context.User.FindFirstValue("tenant_id")
                : context.Request.Headers[TenantHeaderName].FirstOrDefault();

        if (Guid.TryParse(rawTenantId, out var tenantId) && tenantId != Guid.Empty)
            tenantContext.SetTenant(tenantId);

        if (RequiresTenant(context.Request.Path) && !tenantContext.HasTenant)
        {
            tenantContext.SetTenant(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        }

        await next(context);
    }

    private static bool RequiresTenant(PathString path) =>
        path.StartsWithSegments("/api") && !path.StartsWithSegments("/api/v1/auth");
}
