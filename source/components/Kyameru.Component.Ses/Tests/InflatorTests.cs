using System.Linq;
using Amazon.SimpleEmail;
using Kyameru.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Kyameru.Component.Ses.Tests;

public class InflatorTests
{
    [Fact]
    public void RegisterFromComponentThrowsException()
    {
        var serviceCollection = new ServiceCollection();
        var inflator = new Inflator();
        Assert.Throws<Core.Exceptions.RouteNotAvailableException>(() => inflator.RegisterFrom(serviceCollection));
    }

    [Fact]
    public void CreateFromComponentThrowsException()
    {
        var serviceCollection = new ServiceCollection();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var inflator = new Inflator();
        Assert.Throws<RouteNotAvailableException>(() => inflator.CreateFromComponent(new Dictionary<string, string>(), false, serviceProvider));
    }

    [Fact]
    public void CreateAtomicComponentThrowsException()
    {
        var serviceCollection = new ServiceCollection();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var inflator = new Inflator();
        Assert.Throws<RouteNotAvailableException>(() => inflator.CreateAtomicComponent(new Dictionary<string, string>()));
    }

    [Fact]
    public void RegisterToComponentSucceeds()
    {
        var serviceCollection = new ServiceCollection();
        var inflator = new Inflator();
        inflator.RegisterTo(serviceCollection);
        Assert.True(serviceCollection.Contains(typeof(ITo), typeof(SesTo)));
    }

    [Fact]
    public void CreateToComponentSucceeds()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<IAmazonSimpleEmailService>(x =>
        {
            return Substitute.For<IAmazonSimpleEmailService>();
        });
        var inflator = new Inflator();
        inflator.RegisterTo(serviceCollection);
        var builder = serviceCollection.BuildServiceProvider();

        var component = inflator.CreateToComponent(new Dictionary<string, string>(), builder);
        Assert.NotNull(component);
    }
}
