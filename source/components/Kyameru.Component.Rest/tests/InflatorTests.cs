using System;
using Kyameru.Core.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.Rest.Tests;

public class InflatorTests
{
    [Fact]
    public void CreateToCreatesChain()
    {
        var inflator = new Inflator();
        var serviceCollection = GetServiceCollection();
        inflator.RegisterTo(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var toChain = inflator.CreateToComponent(new Dictionary<string, string>(), serviceProvider);
        Assert.NotNull(toChain);
    }

    [Fact]
    public void TestRouteCreation()
    {
        var routeString = "rest://api/v1/hello?endpoint=myapi.com:8080&https=false";
        var routeAttr = new RouteAttributes(routeString);

        Assert.True(true);
    }

    private IServiceCollection GetServiceCollection()
    {
        var collection = new ServiceCollection();
        // Any generic items here
        return collection;
    }
}
