using HospitalSaaS.Application.Common.Interfaces;

namespace HospitalSaaS.Api.Services;

public sealed class RequestContext(IHttpContextAccessor accessor) : IRequestContext
{
    public string? CorrelationId => accessor.HttpContext?.TraceIdentifier;
    public string? IpAddress => accessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
}
