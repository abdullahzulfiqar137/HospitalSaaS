using System.Net;
using System.Net.Mail;
using HospitalSaaS.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HospitalSaaS.Infrastructure.Email;

public sealed class EmailService(IOptions<EmailOptions> options, ILogger<EmailService> logger)
    : IEmailService
{
    public async Task SendAsync(
        string recipient,
        string subject,
        string body,
        CancellationToken cancellationToken
    )
    {
        var settings = options.Value;
        if (!settings.Enabled)
        {
            logger.LogInformation(
                "Email delivery is disabled; message for {Recipient} was skipped",
                recipient
            );
            return;
        }
        if (
            string.IsNullOrWhiteSpace(settings.Host)
            || string.IsNullOrWhiteSpace(settings.FromAddress)
        )
            throw new InvalidOperationException("Email SMTP configuration is incomplete.");

        using var message = new MailMessage
        {
            From = new MailAddress(settings.FromAddress, settings.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };
        message.To.Add(new MailAddress(recipient));
        using var client = new SmtpClient(settings.Host, settings.Port)
        {
            EnableSsl = settings.UseSsl,
            Credentials = string.IsNullOrWhiteSpace(settings.UserName)
                ? CredentialCache.DefaultNetworkCredentials
                : new NetworkCredential(settings.UserName, settings.Password),
        };
        await client.SendMailAsync(message, cancellationToken);
    }
}
