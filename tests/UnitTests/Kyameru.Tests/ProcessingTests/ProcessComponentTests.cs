using System.Threading.Tasks;
using Kyameru.Core.Entities;
using Kyameru.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kyameru.Tests.ProcessingTests;

public class ProcessComponentTests
{
    private readonly ILogger<Route> logger = Substitute.For<ILogger<Route>>();

    [Fact]
    public async Task ProcessComponentByConcrete()
    {
        var serviceCollection = this.GetServiceDescriptors();
        Routable routable = null;
        var processComponent = Substitute.For<IProcessor>();
        processComponent.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            routable = x.Arg<Routable>();
            return Task.CompletedTask;
        });

        Kyameru.Route.From("injectiontest:///mememe")
            .Process(processComponent)
            .To("injectiontest:///somewhere")
            .Build(serviceCollection);

        var provider = serviceCollection.BuildServiceProvider();
        var service = provider.GetService<IHostedService>();
        var thread = TestThread.CreateNew(service.StartAsync, 3);
        thread.StartAndWait();
        await thread.CancelAsync();

        Assert.Equal("Async Injected Test Complete", routable?.Body);
    }

    [Fact]
    public async Task ProcessComponentByDi()
    {
        var serviceCollection = this.GetServiceDescriptors();
        Routable routable = null;
        var processComponent = Substitute.For<IProcessor>();
        processComponent.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            routable = x.Arg<Routable>();
            return Task.CompletedTask;
        });

        serviceCollection.AddTransient<IProcessor>((z) => processComponent);
        Kyameru.Route.From("injectiontest:///mememe")
            .Process<IProcessor>()
            .To("injectiontest:///somewhere")
            .Build(serviceCollection);

        var provider = serviceCollection.BuildServiceProvider();
        var service = provider.GetService<IHostedService>();
        var thread = TestThread.CreateNew(service.StartAsync, 3);
        thread.StartAndWait();
        await thread.CancelAsync();

        Assert.Equal("Async Injected Test Complete", routable?.Body);

        await Task.CompletedTask;
    }

    [Fact]
    public async Task ProcessComponentByAction()
    {
        var serviceCollection = this.GetServiceDescriptors();
        Routable routable = null;

        Kyameru.Route.From("injectiontest:///mememe")
            .Process((Routable x) =>
            {
                routable = x;
            })
            .To("injectiontest:///somewhere")
            .Build(serviceCollection);

        var provider = serviceCollection.BuildServiceProvider();
        var service = provider.GetService<IHostedService>();
        var thread = TestThread.CreateNew(service.StartAsync, 3);
        thread.StartAndWait();
        await thread.CancelAsync();

        Assert.Equal("Async Injected Test Complete", routable?.Body);

        await Task.CompletedTask;
    }

    [Fact]
    public async Task ProcessComponentByFunc()
    {
        var serviceCollection = this.GetServiceDescriptors();
        Routable routable = null;
        Kyameru.Route.From("injectiontest:///mememem")
        .Process(async (Routable x) =>
        {
            routable = x;
            await Task.CompletedTask;
        })
        .To("injectiontest:///somewhere")
        .Build(serviceCollection);
        var provider = serviceCollection.BuildServiceProvider();
        var service = provider.GetService<IHostedService>();
        var thread = TestThread.CreateNew(service.StartAsync, 3);
        thread.StartAndWait();
        await thread.CancelAsync();

        Assert.Equal("Async Injected Test Complete", routable?.Body);

        await Task.CompletedTask;
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
}
