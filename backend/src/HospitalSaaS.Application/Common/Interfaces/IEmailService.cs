namespace HospitalSaaS.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendAsync(
        string recipient,
        string subject,
        string body,
        CancellationToken cancellationToken
    );
}
