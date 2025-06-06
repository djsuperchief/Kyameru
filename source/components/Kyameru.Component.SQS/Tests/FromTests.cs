﻿using Amazon.SQS;
using Amazon.SQS.Model;
using Kyameru.Component.Faker;
using Kyameru.Core.Entities;
using Kyameru.Core.Sys;
using Kyameru.TestUtilities;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NSubstitute.ExceptionExtensions;

namespace Kyameru.Component.Sqs.Tests;

public class FromTests
{
    [Fact]
    public async Task CanProcessMessage()
    {
        var thread = TestThread.CreateDeferred();
        var sqsClient = Substitute.For<IAmazonSQS>();
        var message = string.Empty;
        var randomMessage = Guid.NewGuid().ToString("N");
        sqsClient.ReceiveMessageAsync(Arg.Any<ReceiveMessageRequest>()).ReturnsForAnyArgs(x =>
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
            { "Host", "queue"},
            { "PollTime", "2" }
        });
        from.Setup();
        from.OnActionAsync += async (object sender, RoutableEventData eventData) =>
        {
            message = eventData.Data.Body as string;
            thread.Continue();
            await Task.CompletedTask;
        };
        thread.SetThread(from.StartAsync);
        thread.StartAndWait();
        await from.StopAsync(thread.CancelToken);
        await thread.CancelAsync();

        Assert.Equal(randomMessage, message);

    }

    [Fact]
    public async Task CanProcessMultipleMessages()
    {
        var thread = TestThread.CreateDeferred(15);
        var sqsClient = Substitute.For<IAmazonSQS>();
        var messagesSent = new List<string>();
        var receivedMessages = new List<string>();
        var endOfMessages = false;
        for (var i = 0; i < 5; i++)
        {
            messagesSent.Add(Guid.NewGuid().ToString("N"));
        }

        sqsClient.ReceiveMessageAsync(Arg.Any<ReceiveMessageRequest>()).ReturnsForAnyArgs(x =>
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
            else
            {
                thread.Continue();
            }

            return Task.FromResult(new ReceiveMessageResponse());
        });

        sqsClient.DeleteMessageAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).ReturnsForAnyArgs(x =>
        {
            var message = x[1];
            if (message != null)
            {
                messagesSent.RemoveAt(messagesSent.IndexOf(message.ToString()!));
            }

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
            var message = eventData.Data.Body;
            if (message != null)
            {
                receivedMessages.Add(message.ToString()!);
            }

            if (messagesSent.Count == 0)
            {
                thread.Continue();
            }
            await Task.CompletedTask;
        };

        thread.SetThread(from.StartAsync);
        thread.StartAndWait();
        await from.StopAsync(thread.CancelToken);
        await thread.CancelAsync();

        Assert.Equal(5, receivedMessages.Count);

    }

    [Fact]
    public async Task DeletesMessageFromQueue()
    {
        var thread = TestThread.CreateDeferred(15);
        var sqsClient = Substitute.For<IAmazonSQS>();
        var messagesSent = new List<string>();
        for (var i = 0; i < 5; i++)
        {
            messagesSent.Add(Guid.NewGuid().ToString("N"));
        }

        sqsClient.ReceiveMessageAsync(Arg.Any<ReceiveMessageRequest>()).ReturnsForAnyArgs(x =>
        {
            if (messagesSent.Count > 0)
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
            else
            {
                thread.Continue();
            }

            return Task.FromResult(new ReceiveMessageResponse());
        });

        sqsClient.DeleteMessageAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).ReturnsForAnyArgs(x =>
        {
            var message = x[1];
            if (message != null)
            {
                messagesSent.RemoveAt(messagesSent.IndexOf(message.ToString()!));
            }
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
            if (messagesSent.Count == 0)
            {
                thread.Continue();
            }

            await Task.CompletedTask;
        };
        thread.SetThread(from.StartAsync);
        thread.StartAndWait();
        await from.StopAsync(thread.CancelToken);
        await thread.CancelAsync();
        Assert.Empty(messagesSent);
    }

    [Fact]
    public async Task GetsMessageAttributesAsHeaders()
    {
        var thread = TestThread.CreateDeferred();
        var sqsClient = Substitute.For<IAmazonSQS>();
        Headers? headers = null;
        var randomMessage = Guid.NewGuid().ToString("N");
        sqsClient.ReceiveMessageAsync(Arg.Any<ReceiveMessageRequest>()).ReturnsForAnyArgs(x =>
        {
            var stream = new MemoryStream();
            var message = System.Text.Encoding.UTF8.GetBytes("Will not use");
            stream.Write(message, 0, message.Length);
            var response = Task.FromResult(new ReceiveMessageResponse()
            {


                Messages = new List<Message>() {
                    {
                        new Message() {
                        MessageId ="Test",
                        Body = randomMessage,
                        MessageAttributes = new Dictionary<string, MessageAttributeValue>()
                        {
                            { "Test", new MessageAttributeValue() { StringValue = "Value", DataType = "String" }},
                            { "IsNotHere", new MessageAttributeValue() { BinaryValue = stream, DataType = "Binary" }}
                        }
                    }}
                }
            });

            return response;
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
            headers = eventData.Data.Headers;
            thread.Continue();
            await Task.CompletedTask;
        };

        thread.SetThread(from.StartAsync);
        thread.StartAndWait();
        await from.StopAsync(thread.CancelToken);
        await thread.CancelAsync();

        Assert.Equal("Value", headers!["Test"]);
        Assert.False(headers.ContainsKey("IsNotHere"));
    }

    [Fact]
    public async Task StoppingStopsScan()
    {
        var thread = TestThread.CreateDeferred();
        var sqsClient = Substitute.For<IAmazonSQS>();
        var randomMessage = Guid.NewGuid().ToString("N");

        var from = new SqsFrom(sqsClient);
        from.SetHeaders(new Dictionary<string, string>()
        {
            { "Host", "queue"}
        });
        from.Setup();
        thread.SetThread(from.StartAsync);
        thread.StartAndWait();
        await from.StopAsync(thread.CancelToken);
        await thread.CancelAsync();
        Assert.False(from.IsPolling);
    }

    [Theory]
    [InlineData("https", "false")]
    [InlineData("http", "true")]
    public async Task CanSendByUrl(string protocol, string doHttp)
    {
        var thread = TestThread.CreateDeferred();
        var expectedQueue = $"{protocol}://localhost:4566/000000000000/kyameru-to";
        // Test to make sure a queue url can be used.
        var attributes = new RouteAttributes($"sqs://localhost:4566/000000000000/kyameru-to?PollTime=2&http={doHttp}");
        var headers = attributes.Headers;
        var sqsClient = Substitute.For<IAmazonSQS>();
        var receivedQueue = string.Empty;
        sqsClient.ReceiveMessageAsync(Arg.Any<ReceiveMessageRequest>()).ReturnsForAnyArgs(x =>
        {
            receivedQueue = (x[0] as ReceiveMessageRequest)!.QueueUrl;
            thread.Continue();
            return new ReceiveMessageResponse();
        });

        var from = new SqsFrom(sqsClient);
        from.SetHeaders(headers);
        from.Setup();
        thread.SetThread(from.StartAsync);
        thread.StartAndWait();
        await from.StopAsync(thread.CancelToken);
        await thread.CancelAsync();
        Assert.Equal(expectedQueue, receivedQueue);

    }
}
