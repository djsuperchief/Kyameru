using System.Threading.Tasks;
using Kyameru.Core.Contracts;
using Kyameru.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Kyameru.Tests.ExchangeAndRouter;

public class EventActivationTests : BaseTests
{
    [Fact]
    public void NonEventCompatibleRaisesException()
    {
        var services = GetServiceDescriptors();
        Route.From("test://test")
            .To("test://test")
            .EventTrigger()
            .Build(services);

        var provider = services.BuildServiceProvider();
        var exception = Record.Exception(() => provider.GetRequiredService<IHostedService>());

        Assert.NotNull(exception);
        Assert.Equal("Error activating from chain link.", exception.Message);
        Assert.Equal("The selected from chain does not support event driven triggers", exception.InnerException!.Message);
    }

    [Fact]
    public async Task EventCompatibleDoesNotThrow()
    {
        var services = GetServiceDescriptors();
        var generics = Component.Generic.Builder.Create()
            .WithEventFrom()
            .WithTo(x => { });

        var builder = Route.From("generic:///test")
            .To("generic:///test")
            .EventTrigger();
        var exception = await Record.ExceptionAsync(async () =>
        {
            var service = BuildAndGetServices(builder,  generics);
            var thread = TestThread.CreateDeferred(2);
            thread.SetThread(service.StartAsync);
            thread.StartAndWait();
            await thread.CancelAsync();
        });
        
        Assert.Null(exception);
    }
}