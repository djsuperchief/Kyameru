using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kyameru.Component.Generic;
using Kyameru.Core.Entities;
using Kyameru.Core.Exceptions;
using Kyameru.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kyameru.Tests.BuilderTests;

public class BuilderTests
{
    [Fact]
    public void DuplicateToDependencyRaisesError()
    {
        var route = Route.From("test://test")
        .To("test://test");

        route.AddToDependency<ITestContract, TestImplementation>();

        Assert.Throws<DependencyRegisterException>(() => route.AddToDependency<ITestContract, TestImplementation>());
    }

    [Fact]
    public void AddToDependencyAddsCorrectDependency()
    {
        var route = Route.From("test://test")
        .To("test://test");

        var toId = route.toChainLinks.Last().Id;
        route.AddToDependency<ITestContract, TestImplementation>();

        Assert.Single(route.dependencies);
        Assert.Equal(toId, route.dependencies.Last().Id);
    }

    [Fact]
    public void DuplicateFromDependencyRaisesError()
    {
        var route = Route.From("test://test")
        .To("test://test");

        route.AddFromDependency<ITestContract, TestImplementation>();

        Assert.Throws<DependencyRegisterException>(() => route.AddFromDependency<ITestContract, TestImplementation>());
    }

    [Fact]
    public void AddFromDependencyAddsCorrectly()
    {
        var route = Route.From("test://test")
        .To("test://test");

        var fromId = route.fromChainLink.Id;
        route.AddFromDependency<ITestContract, TestImplementation>();

        Assert.Single(route.dependencies);
        Assert.Equal(fromId, route.dependencies.First().Id);
    }

    [Fact]
    public void RegisterFromDependencyHasRegistered()
    {
        var builder = Route.From("test://test")
        .To("test://test")
        .AddToDependency<ITestContract, TestImplementation>();
        var firstToId = builder.toChainLinks.Last().Id;
        var fromId = builder.fromChainLink.Id;

        builder.To("test://second").AddToDependency<ITestContract, TestImplementation>();
        var secondToId = builder.toChainLinks.Last().Id;

        var serviceDescriptors = new ServiceCollection();
        builder.Build(serviceDescriptors);

        var provider = serviceDescriptors.BuildServiceProvider();

        Assert.NotNull(provider.GetKeyedService<ITestContract>(firstToId));
        Assert.NotNull(provider.GetKeyedService<ITestContract>(secondToId));

    }

    [Fact]
    public async Task ChainLinkGetsIdFromBuilder()
    {
        var serviceDescriptors = GetServiceDescriptors();
        var thread = TestThread.CreateDeferred(5);
        var routable = new Routable(new Dictionary<string, string>(), null);
        Component.Generic.Builder.Create()
        .WithFrom()
        .WithTo((Routable x) =>
        {
            routable = x;
            thread.Continue();
        }).Build(serviceDescriptors);

        var routeBuilder = Route.From("generic:///test")
        .To("generic:///test")
        .Id("identity-pass");

        var fromId = routeBuilder.fromChainLink.Id;
        var toId = routeBuilder.toChainLinks.Last().Id;

        routeBuilder.Build(serviceDescriptors);

        var provider = serviceDescriptors.BuildServiceProvider();
        var service = provider.GetRequiredService<IHostedService>();
        thread.SetThread(service.StartAsync);
        thread.StartAndWait();
        await thread.CancelAsync();

        Assert.Equal(fromId.ToString(), routable.Headers["FromId"]);
        Assert.Equal(toId.ToString(), routable.Headers["ToId"]);
    }

    private IServiceCollection GetServiceDescriptors()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ILogger<Kyameru.Route>>(sp =>
        {
            return Substitute.For<ILogger<Kyameru.Route>>();
        });


        return serviceCollection;
    }
}
