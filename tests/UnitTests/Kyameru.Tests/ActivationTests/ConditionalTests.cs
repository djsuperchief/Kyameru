using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kyameru.Core;
using Kyameru.Core.Entities;
using Kyameru.Tests.Mocks;
using Kyameru.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
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
        var concrete = Substitute.For<IProcessor>();
        var builder = Kyameru.Route.From("test://test")
        .When((Routable x) => x.Body.ToString() == "Test", "testto://test", concrete);
        Assert.Equal(1, builder.ToComponentCount);
    }

    [Fact]
    public void WhenRegistersWithPostProcessingDi()
    {
        var routable = new Routable(new System.Collections.Generic.Dictionary<string, string>(), string.Empty);
        var builder = Kyameru.Route.From("test://test")
        .When<IMyComponent>((Routable x) => x.Body.ToString() == "Test", "testto://test");
        Assert.Equal(1, builder.ToComponentCount);
    }

    [Fact]
    public void WhenRegistersWithPostProcessingReflection()
    {
        var routable = new Routable(new System.Collections.Generic.Dictionary<string, string>(), string.Empty);
        var builder = Kyameru.Route.From("test://test")
        .When((Routable x) => x.Body.ToString() == "Test", "testto://test", "MyComponent");
        Assert.Equal(1, builder.ToComponentCount);
    }

    [Fact]
    public void WhenReflectionRegisters()
    {
        var routable = new Routable(new System.Collections.Generic.Dictionary<string, string>(), string.Empty);
        var builder = Kyameru.Route.From("test://test")
        .When("ConditionalComponent", "testto://test");
        Assert.Equal(1, builder.ToComponentCount);
    }

    [Fact]
    public void WhenReflectionRegistersWithPostProcessingConcrete()
    {
        var routable = new Routable(new System.Collections.Generic.Dictionary<string, string>(), string.Empty);
        var builder = Kyameru.Route.From("test://test")
        .When("ConditionalComponent", "testto://test", "test");
        Assert.Equal(1, builder.ToComponentCount);
    }

    [Fact]
    public void RouteAttributesRegistersCondition()
    {
        var attribute = new RouteAttributes(new DefaultConditional((Routable x) => x.Body.ToString() == "Test"), "testto://test");
        Assert.True(attribute.HasCondition);
    }

    [Fact]
    public async Task WhenComponentExecutesSuccessfully()
    {
        var serviceDescriptors = BuildServices();
        var routable = new Routable(new Dictionary<string, string>(), "Nothing");
        Route.From("test://test?TestName=WhenComponentExecutes")
        .Process((Routable x) =>
        {
            x.SetBody<string>("CondPass");
        })
        .When("ConditionalComponent", "test://test")
        .To("test://test", x =>
        {
            routable = x;
        })
        .Build(serviceDescriptors);
        var provider = serviceDescriptors.BuildServiceProvider();
        var service = provider.GetService<IHostedService>();
        var thread = TestThread.CreateNew(service.StartAsync, 5);
        thread.Start();
        thread.WaitForExecution();
        await thread.CancelAsync();
        Assert.True(routable.Headers.TryGetValue("CondComp", string.Empty) == "true");
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

        var provider = serviceDescriptors.BuildServiceProvider();
        var service = provider.GetService<IHostedService>();
        var thread = TestThread.CreateNew(service.StartAsync, 2);
        thread.Start();
        thread.WaitForExecution();
        await thread.CancelAsync();

        Assert.Equal(expected, executed);

    }

    private IServiceCollection BuildServices()
    {
        var logger = Substitute.For<ILogger<Route>>();
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ILogger<Kyameru.Route>>(sp =>
        {
            return logger;
        });
        serviceCollection.AddTransient<Mocks.IMyComponent, Mocks.MyComponent>();

        return serviceCollection;
    }
}
