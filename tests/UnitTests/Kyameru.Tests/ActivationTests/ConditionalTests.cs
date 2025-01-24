using System;
using System.Threading.Tasks;
using Kyameru.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
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
    public void WhenRegistersWithPostProcessingDelegate()
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
    public void RouteAttributesRegistersCondition()
    {
        var attribute = new RouteAttributes((Routable x) => x.Body.ToString() == "Test", "testto://test");
        Assert.True(attribute.HasCondition);
    }

    [Theory]
    [InlineData("Test", true)]
    [InlineData("Will not execute", false)]
    public void WhenConditionExecutes(string bodyText, bool expected)
    {
        var executed = false;
        var serviceDescriptors = BuildServices();
        Route.From("test://test")
        .When(x => x.Body.ToString() == "Test", "test://test")
        .Build(serviceDescriptors);

        // TODO: write test to run.

        Assert.True(false);

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
