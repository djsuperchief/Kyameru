﻿using Amazon.SimpleNotificationService;
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

    [Fact]
    public async Task RoutableOverridesComponent()
    {
        var snsClient = Substitute.For<IAmazonSimpleNotificationService>();
        var receivedArn = string.Empty;
        snsClient.PublishAsync(Arg.Any<PublishRequest>(), Arg.Any<CancellationToken>()).Returns(x =>
        {
            var request = x[0] as PublishRequest;
            receivedArn = request.TopicArn;
            return new PublishResponse()
            {
                MessageId = request.Message,
                SequenceNumber = "1",
                HttpStatusCode = System.Net.HttpStatusCode.OK
            };
        });

        var to = new SnsTo(snsClient);
        to.SetHeaders(new Dictionary<string, string>() { { "ARN", "dummyarn" } });
        var routable = new Routable(new Dictionary<string, string>()
        {
            { "SNSARN", "Override" }
        }, "test");
        await to.ProcessAsync(routable, default);

        Assert.Equal("Override", receivedArn);
    }

    [Fact]
    public async Task PublishAddsHeaders()
    {
        var snsClient = Substitute.For<IAmazonSimpleNotificationService>();
        var receivedAttribute = string.Empty;
        snsClient.PublishAsync(Arg.Any<PublishRequest>(), Arg.Any<CancellationToken>()).Returns(x =>
        {
            var request = x[0] as PublishRequest;
            receivedAttribute = request.MessageAttributes["TestHeader"].StringValue;
            return new PublishResponse()
            {
                MessageId = request.Message,
                SequenceNumber = "1",
                HttpStatusCode = System.Net.HttpStatusCode.OK
            };
        });

        var to = new SnsTo(snsClient);
        var expected = Guid.NewGuid().ToString("N");
        to.SetHeaders(new Dictionary<string, string>()
        {
            { "ARN", "dummyarn" }
        });
        var routable = new Routable(new Dictionary<string, string>()
        {
            { "TestHeader", expected }
        }, "test");
        await to.ProcessAsync(routable, default);

        Assert.Equal(expected, receivedAttribute);
    }

    [Theory]
    [InlineData("groupdId", "dedupeId")]
    [InlineData("", "")]
    public async Task DeDuplicationAndGroupIdPublishAsExpected(string groupdId, string deduplicationId)
    {

    }
}
