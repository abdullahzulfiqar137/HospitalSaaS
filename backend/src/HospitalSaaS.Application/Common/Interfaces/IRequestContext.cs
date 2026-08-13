namespace HospitalSaaS.Application.Common.Interfaces;

public interface IRequestContext
{
    string? CorrelationId { get; }
    string? IpAddress { get; }
}
