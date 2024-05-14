using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Kyameru.Tests.ProcessingTests;

public class ProcessComponentTests
{
    private readonly Mock<ILogger<Route>> logger = new Mock<ILogger<Route>>();

    [Fact]
    public async Task ProcessComponentByConcrete()
    {
        IServiceCollection serviceCollection = this.GetServiceDescriptors();
        Routable routable = null;
        var processComponent = new Mock<IProcessComponent>();
        processComponent.Setup(x => x.ProcessAsync(It.IsAny<Routable>(), It.IsAny<CancellationToken>())).Callback(async (Routable x, CancellationToken c) =>
            {
                //x.SetHeader
                routable = x;
                await Task.CompletedTask;
            });


        Kyameru.Route.From("injectiontest:///mememe")
            .Process(processComponent.Object)
            .To("injectiontest:///somewhere")
            .Build(serviceCollection);

        IServiceProvider provider = serviceCollection.BuildServiceProvider();
        IHostedService service = provider.GetService<IHostedService>();
        await service.StartAsync(CancellationToken.None);
        await service.StopAsync(CancellationToken.None);

        Assert.Equal("Async Injected Test Complete", routable?.Body);

        await Task.CompletedTask;
    }

    [Fact]
    public async Task ProcessComponentByDi()
    {
        IServiceCollection serviceCollection = this.GetServiceDescriptors();
        Routable routable = null;
        var processComponent = new Mock<IProcessComponent>();
        processComponent.Setup(x => x.ProcessAsync(It.IsAny<Routable>(), It.IsAny<CancellationToken>())).Callback(async (Routable x, CancellationToken c) =>
            {
                //x.SetHeader
                routable = x;
                await Task.CompletedTask;
            });

        serviceCollection.AddTransient<IProcessComponent>((z) => processComponent.Object);
        Kyameru.Route.From("injectiontest:///mememe")
            .Process<IProcessComponent>()
            .To("injectiontest:///somewhere")
            .Build(serviceCollection);

        IServiceProvider provider = serviceCollection.BuildServiceProvider();
        IHostedService service = provider.GetService<IHostedService>();
        await service.StartAsync(CancellationToken.None);
        await service.StopAsync(CancellationToken.None);

        Assert.Equal("Async Injected Test Complete", routable?.Body);

        await Task.CompletedTask;
    }

    [Fact]
    public async Task ProcessComponentByAction()
    {
        IServiceCollection serviceCollection = this.GetServiceDescriptors();
        Routable routable = null;

        Kyameru.Route.From("injectiontest:///mememe")
            .Process((Routable x) =>
            {
                routable = x;
            })
            .To("injectiontest:///somewhere")
            .Build(serviceCollection);

        IServiceProvider provider = serviceCollection.BuildServiceProvider();
        IHostedService service = provider.GetService<IHostedService>();
        await service.StartAsync(CancellationToken.None);
        await service.StopAsync(CancellationToken.None);

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
        IServiceProvider provider = serviceCollection.BuildServiceProvider();
        IHostedService service = provider.GetService<IHostedService>();
        await service.StartAsync(CancellationToken.None);
        await service.StopAsync(CancellationToken.None);

        Assert.Equal("Async Injected Test Complete", routable?.Body);

        await Task.CompletedTask;
    }

    private IServiceCollection GetServiceDescriptors()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ILogger<Kyameru.Route>>(sp =>
        {
            return this.logger.Object;
        });
        serviceCollection.AddTransient<Mocks.IMyComponent, Mocks.MyComponent>();

        return serviceCollection;
    }
}
