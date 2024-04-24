using Amazon.SQS;
using Amazon.SQS.Model;
using Kyameru.Component.Faker;
using Kyameru.Core.Sys;
using NSubstitute;

namespace Kyameru.Component.Sqs.Tests;

public class FromTests
{
    [Fact]
    public async Task CanProcessMessage()
    {
        var resetEvent = new AutoResetEvent(false);
        var sqsClient = Substitute.For<IAmazonSQS>();
        var message = string.Empty;
        var randomMessage = Guid.NewGuid().ToString("N");
        sqsClient.ReceiveMessageAsync(Arg.Any<ReceiveMessageRequest>()).Returns(x =>
        {
            var response = Task.FromResult(new ReceiveMessageResponse()
            {
                Messages = new List<Message>() {
                    {
                        new Message() {
                        MessageId ="Test",
                        Body = randomMessage
                    }}
                }
            });

            return response;
        });

        var from = new SqsFrom(sqsClient);
        from.SetHeaders(new Dictionary<string, string>()
        {
            { "Host", "queue"}
        });
        from.Setup();
        from.OnActionAsync += async (object sender, RoutableEventData eventData) =>
        {
            message = eventData.Data.Body as string;
            resetEvent.Set();
        };
        await from.StartAsync(default);
        resetEvent.WaitOne(5000);
        await from.StopAsync(default);
        Assert.Equal(randomMessage, message);
    }

    [Fact]
    public async Task CanProcessMultipleMessages()
    {
        var resetEvent = new AutoResetEvent(false);
        var sqsClient = Substitute.For<IAmazonSQS>();
        var messagesSent = new List<string>();
        var receivedMessages = new List<string>();
        var endOfMessages = false;
        for (var i = 0; i < 5; i++)
        {
            messagesSent.Add(Guid.NewGuid().ToString("N"));
        }

        sqsClient.ReceiveMessageAsync(Arg.Any<ReceiveMessageRequest>()).Returns(x =>
        {
            endOfMessages = messagesSent.Count == 0;
            if (!endOfMessages)
            {
                var randomMessage = messagesSent.First();
                var response = Task.FromResult(new ReceiveMessageResponse()
                {
                    Messages = new List<Message>()
                    {
                        new()
                        {
                            MessageId = "Test",
                            Body = randomMessage,
                            ReceiptHandle = randomMessage
                        }
                    }
                });
                return response;
            }

            return Task.FromResult(new ReceiveMessageResponse());
        });

        sqsClient.DeleteMessageAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(x =>
        {
            messagesSent.RemoveAt(messagesSent.IndexOf(x[1] as string));
            return Task.FromResult(new DeleteMessageResponse());
        });

        var from = new SqsFrom(sqsClient);
        from.SetHeaders(new Dictionary<string, string>()
        {
            { "Host", "queue"},
            { "PollTime", "2" }
        });
        from.Setup();
        from.OnActionAsync += async (object sender, RoutableEventData eventData) =>
        {
            receivedMessages.Add(eventData.Data.Body as string);
            if (endOfMessages)
            {
                resetEvent.Set();
            }
        };
        await from.StartAsync(default);
        resetEvent.WaitOne(15000);
        await from.StopAsync(default);
        Assert.Equal(5, receivedMessages.Count);

    }

    [Fact]
    public async Task DeletesMessageFromQueue()
    {
        var resetEvent = new AutoResetEvent(false);
        var sqsClient = Substitute.For<IAmazonSQS>();
        var messagesSent = new List<string>();
        var endOfMessages = false;
        for (var i = 0; i < 5; i++)
        {
            messagesSent.Add(Guid.NewGuid().ToString("N"));
        }

        sqsClient.ReceiveMessageAsync(Arg.Any<ReceiveMessageRequest>()).Returns(x =>
        {
            endOfMessages = messagesSent.Count == 0;
            if (!endOfMessages)
            {

                var randomMessage = messagesSent.First();
                var response = Task.FromResult(new ReceiveMessageResponse()
                {
                    Messages = new List<Message>()
                    {
                        new()
                        {
                            MessageId = "Test",
                            Body = randomMessage,
                            ReceiptHandle = randomMessage
                        }
                    }
                });
                return response;
            }

            return Task.FromResult(new ReceiveMessageResponse());
        });

        sqsClient.DeleteMessageAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(x =>
        {
            messagesSent.RemoveAt(messagesSent.IndexOf(x[1] as string));
            return Task.FromResult(new DeleteMessageResponse());
        });

        var from = new SqsFrom(sqsClient);
        from.SetHeaders(new Dictionary<string, string>()
        {
            { "Host", "queue"},
            { "PollTime", "2" }
        });
        from.Setup();
        from.OnActionAsync += async (object sender, RoutableEventData eventData) =>
        {
            if (endOfMessages)
            {
                resetEvent.Set();
            }
        };
        await from.StartAsync(default);
        resetEvent.WaitOne(20000);
        await from.StopAsync(default);
        Assert.Empty(messagesSent);
    }

    [Fact]
    public async Task GetsMessageAttributesAsHeaders()
    {

    }

    [Fact]
    public async Task HostedServiceWorks()
    {

    }

    [Fact]
    public async Task StoppingStopsScan()
    {

    }
}
