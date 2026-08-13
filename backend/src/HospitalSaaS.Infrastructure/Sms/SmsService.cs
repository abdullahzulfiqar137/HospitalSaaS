using System.Net.Http.Headers;
using System.Net.Http.Json;
using HospitalSaaS.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HospitalSaaS.Infrastructure.Sms;

public sealed class SmsService(
    HttpClient client,
    IOptions<SmsOptions> options,
    ILogger<SmsService> logger
) : ISmsService
{
    public async Task SendAsync(
        string phoneNumber,
        string message,
        CancellationToken cancellationToken
    )
    {
        var settings = options.Value;
        if (!settings.Enabled)
        {
            logger.LogInformation(
                "SMS delivery is disabled; message for phone ending {PhoneSuffix} was skipped",
                phoneNumber.Length > 4 ? phoneNumber[^4..] : "****"
            );
            return;
        }
        if (string.IsNullOrWhiteSpace(settings.Endpoint))
            throw new InvalidOperationException("SMS provider endpoint is missing.");

        using var request = new HttpRequestMessage(HttpMethod.Post, settings.Endpoint)
        {
            Content = JsonContent.Create(
                new
                {
                    to = phoneNumber,
                    body = message,
                    sender = settings.Sender,
                }
            ),
        };
        if (!string.IsNullOrWhiteSpace(settings.ApiKey))
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                settings.ApiKey
            );
        using var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
