namespace HospitalSaaS.Infrastructure.Sms;

public sealed class SmsOptions
{
    public const string SectionName = "Sms";
    public bool Enabled { get; init; }
    public string Endpoint { get; init; } = string.Empty;
    public string ApiKey { get; init; } = string.Empty;
    public string Sender { get; init; } = string.Empty;
}
