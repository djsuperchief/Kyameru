using Amazon.SQS;
using Amazon.SQS.Model;
using Kyameru.Core.Entities;
using NSubstitute;

namespace Kyameru.Component.SQS.Tests;

public class ToTests
{
    [Fact]
    public async Task SendMessageIsAsExpected()
    {
        var client = Substitute.For<IAmazonSQS>();
        var to = new SqsTo(client);
        var expectedMessage = "This is a message";
        var queue = "MyQueue";
        to.SetHeaders(new Dictionary<string, string>() { { "Host", queue } });
        var routable = new Routable(new Dictionary<string, string>(), expectedMessage);
        var bodySame = false;
        var queueSame = false;

        client.SendMessageAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(x =>
        {
            bodySame = x[1] as string == expectedMessage;
            queueSame = x[0] as string == queue;
            return new SendMessageResponse();
        });

        await to.ProcessAsync(routable, default);
        Assert.True(bodySame);
        Assert.True(queueSame);
    }
    
    [Fact]
    public async Task MessageOverrideQueueIsCorrect()
    {
        var client = Substitute.For<IAmazonSQS>();
        var to = new SqsTo(client);
        var expectedMessage = "This is a message";
        var queue = "MyQueue";
        to.SetHeaders(new Dictionary<string, string>() { { "Host", "HeaderQueue" } });
        var routable = new Routable(new Dictionary<string, string>(), expectedMessage);
        routable.SetHeader("SQSQueue", queue);
        var receivedQueue = string.Empty;

        client.SendMessageAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(x =>
        {
            receivedQueue = x[0]as string;
            return new SendMessageResponse();
        });

        await to.ProcessAsync(routable, default);
        Assert.Equal(queue, receivedQueue);
    }
}