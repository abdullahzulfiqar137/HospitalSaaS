using System.Diagnostics;
using HospitalSaaS.Application.Common.Interfaces;

namespace HospitalSaaS.Api.Middleware;

public sealed class RequestLoggingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        ITenantContext tenantContext,
        ICurrentUser currentUser,
        ILogger<RequestLoggingMiddleware> logger
    )
    {
        var timer = Stopwatch.StartNew();
        try
        {
            await next(context);
        }
        finally
        {
            timer.Stop();
            logger.LogInformation(
                "HTTP {Method} {Route} returned {StatusCode} in {ElapsedMs}ms for tenant {TenantId} user {UserId}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                timer.ElapsedMilliseconds,
                tenantContext.HasTenant ? tenantContext.TenantId : null,
                currentUser.UserId
            );
        }
    }
}
