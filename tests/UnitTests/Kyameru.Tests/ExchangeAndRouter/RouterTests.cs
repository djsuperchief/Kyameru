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
using Xunit;

namespace Kyameru.Tests.ExchangeAndRouter;

public class RouterTests : BaseTests
{
    [Fact]
    public void CanRegisterChannelForMessageType()
    {
        var router = new KRouter();
        var channel = router.Subscribe("test");
        Assert.NotNull(channel);
    }

    [Fact]
    public void DuplicateRegistersThrowsException()
    {
        var router = new KRouter();
        var channel = router.Subscribe("test");
        var exception = Record.Exception(() => router.Subscribe("test"));
        Assert.NotNull(exception);
        Assert.Equal("A route with the identifier 'test' already exists in the message queues.", exception.Message);
    }

    [Fact]
    public async Task CanPublishMessage()
    {
        var router = new KRouter();
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
            .WithTo(x =>
            {
                eventData = x;
            });
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

    
}