using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Component.Test;
using Kyameru.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Kyameru.Tests.ProcessingTests;

public class AsyncTests
{
    private readonly Mock<ILogger<Route>> logger = new Mock<ILogger<Route>>();
    private readonly Mock<IProcessComponent> processComponent = new Mock<IProcessComponent>();

    [Fact]
    public async Task AsyncRunsAsExpected()
    {
        IServiceCollection serviceCollection = this.GetServiceDescriptors();
        Routable routable = null;
        this.processComponent.Setup(x => x.ProcessAsync(It.IsAny<Routable>(), It.IsAny<CancellationToken>())).Callback(async (Routable x, CancellationToken c) =>
            {
                //x.SetHeader
                routable = x;
                await Task.CompletedTask;
            });

        
        BuildRoute(serviceCollection);

        IServiceProvider provider = serviceCollection.BuildServiceProvider();
        IHostedService service = provider.GetService<IHostedService>();
        await service.StartAsync(CancellationToken.None);
        await service.StopAsync(CancellationToken.None);

        Assert.Equal("Async Injected Test Complete", routable?.Body);

        await Task.CompletedTask;
    }

    [Fact]
    public async Task AsyncRouteRuns()
    {
        IServiceCollection serviceCollection = this.GetServiceDescriptors();
        Routable routable = null;
        this.processComponent.Setup(x => x.ProcessAsync(It.IsAny<Routable>(), It.IsAny<CancellationToken>())).Callback(async (Routable x, CancellationToken c) =>
        {
            x.SetHeader("Process", "ASYNC");
            routable = x;
            await Task.CompletedTask;
        });
        
        BuildRoute(serviceCollection);
        
        IServiceProvider provider = serviceCollection.BuildServiceProvider();
        IHostedService service = provider.GetService<IHostedService>();
        await service.StartAsync(CancellationToken.None);
        await service.StopAsync(CancellationToken.None);
        
        Assert.Contains("FROMASYNC", Component.Injectiontest.GlobalCalls.Calls);
        Assert.Contains("TOASYNC", Component.Injectiontest.GlobalCalls.Calls);
        Assert.Equal("ASYNC", routable.Headers["Process"]);
        Assert.Equal("ASYNC", routable.Headers["FROM"]);
        Assert.Equal("ASYNC", routable.Headers["TO"]);
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

    private void BuildRoute(IServiceCollection serviceCollection)
    {
        Kyameru.Route.From("injectiontest:///mememe")
            .Process(this.processComponent.Object)
            .To("injectiontest:///somewhere")
            .BuildAsync(serviceCollection);
    }
}
