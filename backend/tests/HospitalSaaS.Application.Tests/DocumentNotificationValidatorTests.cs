using HospitalSaaS.Application.Documents;
using HospitalSaaS.Application.Notifications;

namespace HospitalSaaS.Application.Tests;

public sealed class DocumentNotificationValidatorTests
{
    [Fact]
    public async Task Document_Requires_Metadata_And_Allowed_Size()
    {
        var command = new CreateDocumentCommand(Guid.Empty, "", "", 0, "");
        var result = await new CreateDocumentCommandValidator().ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Equal(5, result.Errors.Count);
    }

    [Theory]
    [InlineData("Fax")]
    [InlineData("")]
    public async Task Notification_Rejects_Unsupported_Channel(string channel)
    {
        var command = new CreateNotificationCommand(Guid.NewGuid(), "Subject", "Body", channel);
        var result = await new CreateNotificationCommandValidator().ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.Channel));
    }
}
