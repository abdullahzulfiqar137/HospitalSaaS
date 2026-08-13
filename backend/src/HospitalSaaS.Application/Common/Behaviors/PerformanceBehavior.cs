using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace HospitalSaaS.Application.Common.Behaviors;

public sealed class PerformanceBehavior<TRequest, TResponse>(
    ILogger<PerformanceBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct
    )
    {
        var timer = Stopwatch.StartNew();
        var response = await next(ct);
        timer.Stop();
        if (timer.ElapsedMilliseconds > 500)
            logger.LogWarning(
                "Slow request {RequestType}: {ElapsedMs}ms",
                typeof(TRequest).Name,
                timer.ElapsedMilliseconds
            );
        return response;
    }
}
