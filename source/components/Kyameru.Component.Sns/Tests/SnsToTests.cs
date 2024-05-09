using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Kyameru.Core.Entities;
using NSubstitute;

namespace Kyameru.Component.Sns.Tests;

public class SnsToTests
{
    [Fact]
    public async Task CanPublishMessageSimple()
    {
        var snsClient = Substitute.For<IAmazonSimpleNotificationService>();
        snsClient.PublishAsync(Arg.Any<PublishRequest>(), Arg.Any<CancellationToken>()).Returns(x =>
        {
            var request = x[0] as PublishRequest;

            return new PublishResponse()
            {
                MessageId = request.Message,
                SequenceNumber = "1",
                HttpStatusCode = System.Net.HttpStatusCode.OK
            };
        });
        var publishedMessage = string.Empty;
        var expectedMessage = "SNS Message";

        var to = new SnsTo(snsClient);
        to.SetHeaders(new Dictionary<string, string>() { { "ARN", "dummyarn" } });
        var routable = new Routable(new Dictionary<string, string>(), expectedMessage);
        await to.ProcessAsync(routable, default);

        Assert.Equal(expectedMessage, routable.Headers["SNSMessageId"]);
    }
}
