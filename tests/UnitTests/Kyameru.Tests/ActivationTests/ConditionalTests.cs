using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Entities;
using Kyameru.Tests.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Kyameru.Tests.ActivationTests;

/// <summary>
/// Conditional route tests.
/// </summary>
public class ConditionalTests
{
    [Fact]
    public void WhenRegistersToComponent()
    {
        var builder = Kyameru.Route.From("test://test")
        .When((Routable x) => x.Body.ToString() == "Test", "testto://test");
        Assert.Equal(1, builder.ToComponentCount);
    }

    [Fact]
    public void WhenRegistersWithAsyncPostProcessingDelegate()
    {
        var routable = new Routable(new System.Collections.Generic.Dictionary<string, string>(), string.Empty);
        var builder = Kyameru.Route.From("test://test")
        .When((Routable x) => x.Body.ToString() == "Test", "testto://test", async (Routable x) =>
        {
            routable = x;

            await Task.CompletedTask;
        });
        Assert.Equal(1, builder.ToComponentCount);
    }

    [Fact]
    public void WhenRegistersWithPostProcessingDelegate()
    {
        var routable = new Routable(new System.Collections.Generic.Dictionary<string, string>(), string.Empty);
        var builder = Kyameru.Route.From("test://test")
        .When((Routable x) => x.Body.ToString() == "Test", "testto://test", (Routable x) =>
        {
            routable = x;
        });
        Assert.Equal(1, builder.ToComponentCount);
    }

    [Fact]
    public void WhenRegistersWithPostProcessingConcrete()
    {
        var routable = new Routable(new System.Collections.Generic.Dictionary<string, string>(), string.Empty);
        var concrete = new Mock<IProcessComponent>();
        var builder = Kyameru.Route.From("test://test")
        .When((Routable x) => x.Body.ToString() == "Test", "testto://test", concrete.Object);
        Assert.Equal(1, builder.ToComponentCount);
    }

    [Fact]
    public void WhenRegistersWithPostProcessingDi()
    {
        var routable = new Routable(new System.Collections.Generic.Dictionary<string, string>(), string.Empty);
        var concrete = new Mock<IProcessComponent>();
        var builder = Kyameru.Route.From("test://test")
        .When<IMyComponent>((Routable x) => x.Body.ToString() == "Test", "testto://test");
        Assert.Equal(1, builder.ToComponentCount);
    }

    [Fact]
    public void WhenRegistersWithPostProcessingReflection()
    {
        var routable = new Routable(new System.Collections.Generic.Dictionary<string, string>(), string.Empty);
        var concrete = new Mock<IProcessComponent>();
        var builder = Kyameru.Route.From("test://test")
        .When((Routable x) => x.Body.ToString() == "Test", "testto://test", "MyComponent");
        Assert.Equal(1, builder.ToComponentCount);
    }

    [Fact]
    public void RouteAttributesRegistersCondition()
    {
        var attribute = new RouteAttributes((Routable x) => x.Body.ToString() == "Test", "testto://test");
        Assert.True(attribute.HasCondition);
    }

    [Theory]
    [InlineData("Test", true)]
    [InlineData("Will not execute", false)]
    public async Task WhenConditionExecutes(string bodyText, bool expected)
    {
        var executed = false;
        var serviceDescriptors = BuildServices();
        Route.From("test://test?TestName=WhenConditionExecutes")
        .Process((Routable x) =>
        {
            x.SetBody<string>(bodyText);
        })
        .When(x => x.Body.ToString() == "Test", "test://test", (Routable x) =>
        {
            executed = x.Headers.TryGetValue("ToExecuted", string.Empty) == "true";
        })
        .Build(serviceDescriptors);

        IServiceProvider provider = serviceDescriptors.BuildServiceProvider();
        IHostedService service = provider.GetService<IHostedService>();
        await service.StartAsync(CancellationToken.None);
        await service.StopAsync(CancellationToken.None);

        Assert.Equal(expected, executed);

    }

    private IServiceCollection BuildServices()
    {
        var logger = new Mock<ILogger<Route>>();
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ILogger<Kyameru.Route>>(sp =>
        {
            return logger.Object;
        });
        serviceCollection.AddTransient<Mocks.IMyComponent, Mocks.MyComponent>();

        return serviceCollection;
    }
}
