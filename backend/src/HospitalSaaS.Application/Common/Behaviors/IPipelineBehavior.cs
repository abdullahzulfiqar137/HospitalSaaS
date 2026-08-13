namespace HospitalSaaS.Application.Common.Behaviors;

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>(
    CancellationToken cancellationToken
);

public interface IPipelineBehavior<in TRequest, TResponse>
{
    Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    );
}
