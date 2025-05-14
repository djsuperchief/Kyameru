using System.Threading.Tasks;
using Kyameru.Core.Entities;
using Kyameru.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kyameru.Tests.ProcessingTests;

public class AsyncTests
{
    private readonly ILogger<Route> logger = Substitute.For<ILogger<Route>>();

    [Fact]
    public async Task AsyncRunsAsExpected()
    {
        var processComponent = Substitute.For<IProcessor>();
        var serviceCollection = this.GetServiceDescriptors();
        Routable routable = null;
        processComponent.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            routable = x.Arg<Routable>();
            return Task.CompletedTask;
        });

        BuildRoute(serviceCollection, processComponent);

        var provider = serviceCollection.BuildServiceProvider();
        var service = provider.GetService<IHostedService>();
        var thread = TestThread.CreateNew(service.StartAsync, 3);
        thread.StartAndWait();
        await thread.CancelAsync();

        Assert.Equal("Async Injected Test Complete", routable?.Body);
    }

    [Fact]
    public async Task AsyncRouteRuns()
    {
        var processComponent = Substitute.For<IProcessor>();
        var serviceCollection = this.GetServiceDescriptors();
        Routable routable = null;
        processComponent.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            routable = x.Arg<Routable>();
            routable.SetHeader("Process", "ASYNC");
            return Task.CompletedTask;
        });

        BuildRoute(serviceCollection, processComponent);

        var provider = serviceCollection.BuildServiceProvider();
        var service = provider.GetService<IHostedService>();
        var thread = TestThread.CreateNew(service.StartAsync, 3);
        thread.StartAndWait();
        await thread.CancelAsync();

        Assert.Equal("ASYNC", routable.Headers["Process"]);
        Assert.Equal("ASYNC", routable.Headers["FROM"]);
        Assert.Equal("ASYNC", routable.Headers["TO"]);
    }


    private IServiceCollection GetServiceDescriptors()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ILogger<Kyameru.Route>>(sp =>
        {
            return this.logger;
        });
        serviceCollection.AddTransient<Mocks.IMyComponent, Mocks.MyComponent>();

        return serviceCollection;
    }

    private void BuildRoute(IServiceCollection serviceCollection, IProcessor processComponent)
    {
        Kyameru.Route.From("injectiontest:///mememe")
            .Process(processComponent)
            .To("injectiontest:///somewhere")
            .Build(serviceCollection);
    }
}
