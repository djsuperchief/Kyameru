using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Kyameru.Component.Generic;
using Kyameru.Core.Comms;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Kyameru.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kyameru.Tests.ExchangeAndRouter;

public class RouterTests : BaseTests
{
    [Fact]
    public void CanRegisterChannelForMessageType()
    {
        var loggerMock = Substitute.For<ILogger<KRouter>>();
        var router = new KRouter(loggerMock);
        var channel = router.Subscribe("test");
        Assert.NotNull(channel);
    }

    [Fact]
    public void DuplicateRegistersThrowsException()
    {
        var loggerMock = Substitute.For<ILogger<KRouter>>();
        var router = new KRouter(loggerMock);
        var channel = router.Subscribe("test");
        var exception = Record.Exception(() => router.Subscribe("test"));
        Assert.NotNull(exception);
        Assert.Equal("A route with the identifier 'test' already exists in the message queues.", exception.Message);
    }

    [Fact]
    public async Task CanPublishMessage()
    {
        var loggerMock = Substitute.For<ILogger<KRouter>>();
        var router = new KRouter(loggerMock);
        var channel = router.Subscribe("test");

        var publishMessage = CommsMessage.Create("test", new TestMessage("Hello world"));
        await router.PublishAsync(publishMessage, CancellationToken.None);
        CommsMessage receivedMessage = null;
        await foreach (var message in channel.ReadAllAsync())
        {
            receivedMessage = message;
            break;
        }

        Assert.Equal("Hello world", ((TestMessage)receivedMessage.Data).Name);
    }

    [Fact]
    public async Task CanPublishAndProcessMessage()
    {
        var thread = TestThread.CreateDeferred(10);
        Routable eventData = null;
        var generics = Component.Generic.Builder.Create()
            .WithEventFrom()
            .WithTo(x => { eventData = x; });
        var builder = Route.From("generic:///test")
            .To("generic:///test")
            .Id("test");
        builder.EventTrigger();

        var serviceProvider = BuildAndGetProvider(builder, generics);
        var exchange = serviceProvider.GetRequiredService<IKExchange>();
        var route = serviceProvider.GetRequiredService<IHostedService>();
        thread.SetThread(route.StartAsync);
        thread.Start();
        await exchange.PublishMessageAsync("test", GenericMessage.Create("Test Message"), thread.CancelToken);
        thread.WaitForExecution();
        await thread.CancelAsync();

        Assert.Equal("Test Message", eventData.Body);
    }

    [Fact]
    public async Task MultipleRoutesProcessMessagesCorrectly()
    {
        var routeOne = TestThread.CreateDeferred(20);
        var routeTwo = TestThread.CreateDeferred(20);
        var received = new Dictionary<string, string>();

        var serviceDescriptors = GetServiceDescriptors();
        Component.Generic.Builder.Create()
            .WithEventFrom()
            .WithTo(x => { })
            .Build(serviceDescriptors);

        Route.From("generic:///test?name=One")
            .To("generic:///test", x =>
            {
                received.Add("first", x.Body.ToString());
                routeOne.Continue();
            })
            .Id("first")
            .EventTrigger()
            .Build(serviceDescriptors);
        Route.From("generic:///test?name=Two")
            .To("generic:///test", x =>
            {
                received.Add("second", x.Body.ToString());
                routeTwo.Continue();
            })
            .Id("second")
            .EventTrigger()
            .Build(serviceDescriptors);

        var provider = serviceDescriptors.BuildServiceProvider();
        var exchange = provider.GetRequiredService<IKExchange>();
        var services = provider.GetServices<IHostedService>();
        routeOne.SetThread(services.ElementAt(0).StartAsync);
        routeTwo.SetThread(services.ElementAt(1).StartAsync);
        await exchange.PublishMessageAsync("first", GenericMessage.Create("Message For One"), routeOne.CancelToken);
        await exchange.PublishMessageAsync("second", GenericMessage.Create("Message For Two"), routeTwo.CancelToken);
        routeOne.StartAndWait();
        routeTwo.StartAndWait();

        Assert.Equal(2, received.Count);
        Assert.Equal("Message For One", received["first"]);
        Assert.Equal("Message For Two", received["second"]);
    }

    [Fact]
    public async Task EventRouteLogsError()
    {
        var routeOne = TestThread.CreateDeferred(15);
        var logger = Substitute.For<ILogger<Route>>();
        var receivedMessage = string.Empty;
        var serviceDescriptors = GetServiceDescriptors(logger);
        var received = new Dictionary<string, string>();
        logger.WhenForAnyArgs(x => x.Log(default, default, default, default, default))
            .Do(callInfo =>
            {
                var level = callInfo.ArgAt<LogLevel>(0);
                var eventId = callInfo.ArgAt<EventId>(1);
                var state = callInfo.ArgAt<object>(2);
                var ex = callInfo.ArgAt<Exception>(3);

                var formatter = (Delegate)callInfo.Args()[4];
                var message = formatter.DynamicInvoke(state, ex);
                if (level == LogLevel.Error)
                {
                    receivedMessage += message;
                }
            });
        Component.Generic.Builder.Create()
            .WithEventFrom(() => throw new NotImplementedException())
            .WithTo(x => { })
            .Build(serviceDescriptors);
        Route.From("generic:///test?name=One")
            .To("generic:///test", x =>
            {
                received.Add("first", x.Body.ToString());
                routeOne.Continue();
            })
            .Id("first")
            .EventTrigger()
            .Build(serviceDescriptors);

        var provider = serviceDescriptors.BuildServiceProvider();
        var exchange = provider.GetRequiredService<IKExchange>();
        var services = provider.GetServices<IHostedService>();
        routeOne.SetThread(services.ElementAt(0).StartAsync);
        await exchange.PublishMessageAsync("first", GenericMessage.Create("Message For One"), routeOne.CancelToken);
        routeOne.StartAndWait();

        Assert.Equal(0, received.Count);
        foreach(var x in logger.ReceivedCalls())
        {
            string test = "test";
        }
    }
}