namespace HospitalSaaS.Application.Common.Messaging;

public interface IRequestDispatcher
{
    Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequest<TResponse>;
}
