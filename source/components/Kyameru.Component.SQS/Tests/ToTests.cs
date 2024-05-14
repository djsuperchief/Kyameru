using Amazon.SQS;
using Amazon.SQS.Model;
using Kyameru.Core.Entities;
using NSubstitute;

namespace Kyameru.Component.Sqs.Tests;

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

        client.SendMessageAsync(Arg.Any<SendMessageRequest>(), Arg.Any<CancellationToken>()).Returns(x =>
        {
            var message = x[0] as SendMessageRequest;
            bodySame = message!.MessageBody == expectedMessage;
            queueSame = message.QueueUrl == queue;
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

        client.SendMessageAsync(Arg.Any<SendMessageRequest>(), Arg.Any<CancellationToken>()).Returns(x =>
        {
            receivedQueue = (x[0] as SendMessageRequest)!.QueueUrl;
            return new SendMessageResponse();
        });

        await to.ProcessAsync(routable, default);
        Assert.Equal(queue, receivedQueue);
    }

    [Fact]
    public async Task SendMessageSyncIsAsExpected()
    {
        var resetEvent = new AutoResetEvent(false);
        var client = Substitute.For<IAmazonSQS>();
        var to = new SqsTo(client);
        var expectedMessage = "This is a message";
        var queue = "MyQueue";
        to.SetHeaders(new Dictionary<string, string>() { { "Host", queue } });
        var routable = new Routable(new Dictionary<string, string>(), expectedMessage);
        var bodySame = false;
        var queueSame = false;

        client.SendMessageAsync(Arg.Any<SendMessageRequest>(), Arg.Any<CancellationToken>()).Returns(x =>
        {
            var message = x[0] as SendMessageRequest;
            bodySame = message!.MessageBody == expectedMessage;
            queueSame = message.QueueUrl == queue;
            resetEvent.Set();
            return new SendMessageResponse();
        });

        await to.ProcessAsync(routable, default);
        resetEvent.WaitOne(5000);
        Assert.True(bodySame);
        Assert.True(queueSame);
    }

    [Fact]
    public async Task HeadersFromRoutableInMessageHeader()
    {
        var receivedHeaders = new Dictionary<string, string>();
        var headers = new Dictionary<string, string>()
        {
            { "RandomHeader", "RandomValue" },
            { "SQSQueue", "Queue" }
        };
        var routable = new Routable(headers, "Data");
        var client = Substitute.For<IAmazonSQS>();
        client.SendMessageAsync(Arg.Any<SendMessageRequest>(), Arg.Any<CancellationToken>()).Returns(x =>
        {
            var request = x[0] as SendMessageRequest;
            receivedHeaders = request?.MessageAttributes.ToDictionary(x => x.Key, x => x.Value.StringValue);
            var response = new SendMessageResponse();
            response.MessageId = Guid.NewGuid().ToString("N");
            return response;
        });

        var to = new SqsTo(client);
        to.SetHeaders(new Dictionary<string, string>()
        {
            { "Host", "my-queue" }
        });
        await to.ProcessAsync(routable, default);
        // SQSQueue and message id will not be there.
        Assert.Equal(receivedHeaders.Count, routable.Headers.Count - 2);
        Assert.All(receivedHeaders, x => Assert.Equal(x.Value, routable.Headers[x.Key]));
    }

    [Theory]
    [InlineData("https", "false")]
    [InlineData("http", "true")]
    public async Task CanSendByUrl(string protocol, string doHttp)
    {
        var expectedQueue = $"{protocol}://localhost:4566/000000000000/kyameru-to";
        // Test to make sure a queue url can be used.
        var attributes = new RouteAttributes($"sqs://localhost:4566/000000000000/kyameru-to?PollTime=2&http={doHttp}");
        var headers = attributes.Headers;
        var sqsClient = Substitute.For<IAmazonSQS>();
        var resetEvent = new AutoResetEvent(false);
        var receivedQueue = string.Empty;
        sqsClient.SendMessageAsync(Arg.Any<SendMessageRequest>(), Arg.Any<CancellationToken>()).Returns(x =>
        {
            receivedQueue = (x[0] as SendMessageRequest)!.QueueUrl;
            resetEvent.Set();
            return new SendMessageResponse();
        });

        var to = new SqsTo(sqsClient);
        to.SetHeaders(headers);
        var messageHeaders = new Dictionary<string, string>()
        {
            { "RandomHeader", "RandomValue" }
        };
        var routable = new Routable(messageHeaders, "Data");
        await to.ProcessAsync(routable, default);
        Assert.Equal(expectedQueue, receivedQueue);
    }

    [Theory]
    [InlineData("https", "false", "https://localhost:4566/000000000000/kyameru-to")]
    [InlineData("http", "true", "http://localhost:4566/000000000000/kyameru-to")]
    [InlineData("https", "false", "localhost:4566/000000000000/kyameru-to")]
    [InlineData("http", "true", "localhost:4566/000000000000/kyameru-to")]
    public async Task CanSendByUrlInMessage(string protocol, string doHttp, string address)
    {
        var expectedQueue = $"{protocol}://localhost:4566/000000000000/kyameru-to";
        // Test to make sure a queue url can be used.
        var attributes = new RouteAttributes($"sqs://kyameru-to?PollTime=2&http={doHttp}");
        var headers = attributes.Headers;
        var sqsClient = Substitute.For<IAmazonSQS>();
        var resetEvent = new AutoResetEvent(false);
        var receivedQueue = string.Empty;
        sqsClient.SendMessageAsync(Arg.Any<SendMessageRequest>(), Arg.Any<CancellationToken>()).Returns(x =>
        {
            receivedQueue = (x[0] as SendMessageRequest)!.QueueUrl;
            resetEvent.Set();
            return new SendMessageResponse();
        });

        var to = new SqsTo(sqsClient);
        to.SetHeaders(headers);
        var messageHeaders = new Dictionary<string, string>()
        {
            { "RandomHeader", "RandomValue" },
            { "SQSQueue", address }
        };
        var routable = new Routable(messageHeaders, "Data");
        await to.ProcessAsync(routable, default);
        Assert.Equal(expectedQueue, receivedQueue);
    }
}