using System;
using System.Threading.Tasks;
using Kyameru.Core.Entities;
using Kyameru.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kyameru.Tests.ActivationTests;

public class InjectedComponentsTests
{
    [Fact]
    public async Task CanActivateAndRun()
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
        var thread = TestThread.CreateNew(service.StartAsync, 2);
        thread.Start();
        thread.WaitForExecution();
        await thread.CancelAsync();

        Assert.Equal("Async Injected Test Complete", routable?.Body);
    }

    [Theory]
    [InlineData("from", "Error activating from chain link.")]
    [InlineData("to", "Error activating to chain link.")]
    public async Task CanComponentsStart(string componentToError, string expected)
    {
        var serviceCollection = this.GetServiceDescriptors();
        var processComponent = Substitute.For<IProcessor>();
        var fromHeaders = componentToError == "from" ? "?WillError=true" : string.Empty;
        var toHeaders = componentToError == "to" ? "?WillError=true" : string.Empty;
        var actual = string.Empty;
        try
        {
            Kyameru.Route.From($"injectiontest:///mememe{fromHeaders}")
                .Process(processComponent)
                .To($"injectiontest:///somewhere{toHeaders}")
                .Build(serviceCollection);

            var provider = serviceCollection.BuildServiceProvider();
            var service = provider.GetService<IHostedService>();
            var thread = TestThread.CreateNew(service.StartAsync, 2);
            thread.Start();
            thread.WaitForExecution();
            await thread.CancelAsync();
        }
        catch (Exception ex)
        {
            actual = ex.Message;
        }

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task CanActivateProcessingByDomain()
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
            .Process("Mocks.MyComponent")
            .Process(processComponent)
            .To("injectiontest:///somewhere")
            .Build(serviceCollection);
        var provider = serviceCollection.BuildServiceProvider();
        var service = provider.GetService<IHostedService>();
        var thread = TestThread.CreateNew(service.StartAsync, 2);
        thread.Start();
        thread.WaitForExecution();
        await thread.CancelAsync();

        Assert.Equal("Yes", routable.Headers["ComponentRan"]);
    }

    private IServiceCollection GetServiceDescriptors()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ILogger<Kyameru.Route>>(sp =>
        {
            return Substitute.For<ILogger<Kyameru.Route>>();
        });
        serviceCollection.AddTransient<Mocks.IMyComponent, Mocks.MyComponent>();

        return serviceCollection;
    }
}
