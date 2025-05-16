using Amazon.SQS;
using Amazon.SQS.Model;
using Kyameru.Component.Faker;
using Kyameru.Core.Entities;
using Kyameru.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Kyameru.Component.Sqs.Tests;

public class FullTests
{
    [Fact]
    public async Task ChainedToSyncWorksAsExpected()
    {
        var serviceCollection = GetServiceDescriptors();
        var thread = TestThread.CreateDeferred();
        var routable = new Routable(new Dictionary<string, string>(), "Test");
        var extractor = Substitute.For<IExtractor>();
        extractor.When(x => x.SetRoutable(Arg.Any<Routable>())).Do(x =>
        {
            routable = x[0] as Routable;
        });
        serviceCollection.AddTransient<IExtractor>(x => extractor);
        Kyameru.Route.From("faker://who/cares")
            .To("sqs://queue")
            .To("faker://who/cares", x =>
            {
                thread.Continue();
            })
            .Id("FakerSyncTest")
            .Build(serviceCollection);

        IServiceProvider provider = serviceCollection.BuildServiceProvider();
        IHostedService service = provider.GetRequiredService<IHostedService>();
        thread.SetThread(service.StartAsync);
        thread.StartAndWait();
        await service.StopAsync(thread.CancelToken);
        await thread.CancelAsync();

        Assert.Equal("Faker Test", routable.Headers["SQSMessageId"]);
    }


    [Fact]
    public async Task FullFromAndToWorksAsExpected()
    {
        var expectedMessage = Guid.NewGuid().ToString("N");
        var serviceCollection = GetServiceDescriptors(expectedMessage);
        var thread = TestThread.CreateDeferred();
        var routable = new Routable(new Dictionary<string, string>(), "Test");
        var extractor = Substitute.For<IExtractor>();

        extractor.When(x => x.SetRoutable(Arg.Any<Routable>())).Do(x =>
        {
            routable = x[0] as Routable;
        });
        serviceCollection.AddTransient<IExtractor>(x => extractor);
        Kyameru.Route.From("sqs://queue?PollTime=2")
            .To("sqs://queue")
            .To("faker://who/cares", x =>
            {
                thread.Continue();
            })
            .Id("FakerSyncTest")
            .Build(serviceCollection);
        IServiceProvider provider = serviceCollection.BuildServiceProvider();
        IHostedService service = provider.GetRequiredService<IHostedService>();
        thread.SetThread(service.StartAsync);
        thread.SetThread(service.StartAsync);
        thread.StartAndWait();
        await service.StopAsync(thread.CancelToken);
        await thread.CancelAsync();

        Assert.Equal("Faker Test", routable.Headers["SQSMessageId"]);
        Assert.Equal("MessageHeader", routable.Headers["Full Test"]);
        Assert.Equal(expectedMessage, routable.Body as string);
    }

    private IServiceCollection GetServiceDescriptors(string message = "Test")
    {
        var sqsClient = Substitute.For<IAmazonSQS>();
        var messages = new List<string>()
        {
            { message }
        };
        sqsClient.SendMessageAsync(Arg.Any<SendMessageRequest>(), Arg.Any<CancellationToken>()).ReturnsForAnyArgs(x =>
        {
            var response = new SendMessageResponse();
            response.MessageId = "Faker Test";
            // simulated wait
            Thread.Sleep(1000);
            return response;
        });

        sqsClient.ReceiveMessageAsync(Arg.Any<ReceiveMessageRequest>()).ReturnsForAnyArgs(x =>
        {
            if (messages.Count > 0)
            {

                var randomMessage = messages.First();
                var response = Task.FromResult(new ReceiveMessageResponse()
                {
                    Messages = new List<Message>()
                    {
                        new()
                        {
                            MessageId = "Test",
                            Body = randomMessage,
                            ReceiptHandle = randomMessage,
                            MessageAttributes = new Dictionary<string, MessageAttributeValue>()
                            {
                                { "Full Test", new () { DataType = "String", StringValue = "MessageHeader" } }
                            }
                        }
                    }
                });
                return response;
            }

            return Task.FromResult(new ReceiveMessageResponse());
        });


        sqsClient.DeleteMessageAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(x =>
        {
            var message = x[1];
            if (message != null)
            {
                messages.RemoveAt(messages.IndexOf(message.ToString()!));
            }
            return Task.FromResult(new DeleteMessageResponse());
        });

        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ILogger<Kyameru.Route>>(sp =>
        {
            return Substitute.For<ILogger<Kyameru.Route>>();
        });
        serviceCollection.AddTransient<IAmazonSQS>(x => sqsClient);

        return serviceCollection;
    }
}