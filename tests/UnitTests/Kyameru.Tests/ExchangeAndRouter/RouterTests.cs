using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Kyameru.Core.Comms;
using Xunit;

namespace Kyameru.Tests.ExchangeAndRouter;

public class RouterTests
{
    [Fact]
    public void CanRegisterChannelForMessageType()
    {
        var router = new KRouter();
        var channel = router.Subscribe<TestMessage>();
        Assert.NotNull(channel);
    }

    [Fact]
    public async Task CanPublishMessage()
    {
        var router = new KRouter();
        var channel = router.Subscribe<TestMessage>();

        await router.Publish<TestMessage>(new TestMessage("Hello world"), CancellationToken.None);
        TestMessage receivedMessage = null;
        await foreach (var message in channel.ReadAllAsync())
        {
            receivedMessage = message;
            break;
        }
        
        Assert.Equal(receivedMessage.Data, "Hello world");
    }
}