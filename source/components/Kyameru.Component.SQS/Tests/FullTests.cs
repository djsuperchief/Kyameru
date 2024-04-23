using Amazon.SQS;
using Amazon.SQS.Model;
using Kyameru.Component.Faker;
using Kyameru.Core.Entities;
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
        var routable = new Routable(new Dictionary<string, string>(), "Test");
        var extractor = Substitute.For<IExtractor>();
        extractor.When(x => x.SetRoutable(Arg.Any<Routable>())).Do(x =>
        {
            routable = x[0] as Routable;
        });
        serviceCollection.AddTransient<IExtractor>(x => extractor);
        Kyameru.Route.From("faker://who/cares")
            .To("sqs://queue")
            .To("faker://who/cares")
            .Id("FakerSyncTest")
            .Build(serviceCollection);

        IServiceProvider provider = serviceCollection.BuildServiceProvider();
        IHostedService service = provider.GetService<IHostedService>();
        await service.StartAsync(CancellationToken.None);
        await service.StopAsync(CancellationToken.None);

        Assert.Equal("Faker Test", routable.Headers["SQSMessageId"]);

        await Task.CompletedTask;
    }

    [Fact]
    public async Task ChainedToAsyncSyncWorksAsExpected()
    {
        var serviceCollection = GetServiceDescriptors();
        var routable = new Routable(new Dictionary<string, string>(), "Test");
        var extractor = Substitute.For<IExtractor>();
        extractor.When(x => x.SetRoutable(Arg.Any<Routable>())).Do(x =>
        {
            routable = x[0] as Routable;
        });
        serviceCollection.AddTransient<IExtractor>(x => extractor);
        Kyameru.Route.From("faker://who/cares")
            .To("sqs://queue")
            .To("faker://who/cares")
            .Id("FakerSyncTest")
            .BuildAsync(serviceCollection);

        IServiceProvider provider = serviceCollection.BuildServiceProvider();
        IHostedService service = provider.GetService<IHostedService>();
        await service.StartAsync(CancellationToken.None);
        await service.StopAsync(CancellationToken.None);

        Assert.Equal("Faker Test", routable.Headers["SQSMessageId"]);

        await Task.CompletedTask;
    }

    private IServiceCollection GetServiceDescriptors()
    {
        var sqsClient = Substitute.For<IAmazonSQS>();
        sqsClient.SendMessageAsync(Arg.Any<SendMessageRequest>(), Arg.Any<CancellationToken>()).Returns(x =>
        {
            var response = new SendMessageResponse();
            response.MessageId = "Faker Test";
            // simulated wait
            Thread.Sleep(1000);
            return response;
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