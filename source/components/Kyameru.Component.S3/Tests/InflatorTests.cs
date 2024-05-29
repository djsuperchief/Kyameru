using Amazon.S3;
using Kyameru.Core;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;


namespace Kyameru.Component.S3.Tests;

public class InflatorTests
{
    [Fact]
    public void RegisterFromThrowsNotImplemented()
    {
        var serviceCollection = new ServiceCollection();
        var inflator = new Inflator();
        Assert.Throws<RouteNotAvailableException>(() => inflator.RegisterFrom(serviceCollection));
    }

    [Fact]
    public void CreateFromThrowsNotImplemented()
    {
        var serviceCollection = new ServiceCollection();
        var provider = serviceCollection.BuildServiceProvider();
        var inflator = new Inflator();
        Assert.Throws<RouteNotAvailableException>(() => inflator.CreateFromComponent(new Dictionary<string, string>(), false, provider));
    }

    [Fact]
    public void CreateAtomicThrowsNotImplemented()
    {
        var serviceCollection = new ServiceCollection();
        var provider = serviceCollection.BuildServiceProvider();
        var inflator = new Inflator();
        Assert.Throws<RouteNotAvailableException>(() => inflator.CreateAtomicComponent(new Dictionary<string, string>()));
    }

    [Fact]
    public void RegisterToComponentSucceeds()
    {
        var serviceCollection = new ServiceCollection();
        var inflator = new Inflator();
        inflator.RegisterTo(serviceCollection);
        Assert.True(serviceCollection.Contains(typeof(ITo), typeof(S3To)));
    }

    [Fact]
    public void CreateToComponentSucceeds()
    {
        var serviceCollection = new ServiceCollection();
        var mockS3 = Substitute.For<IAmazonS3>();
        serviceCollection.AddTransient<IAmazonS3>((IServiceProvider sp) =>
        {
            return mockS3;
        });
        var headers = new Dictionary<string, string>() {
            { "Host", "Test" }
        };
        var inflator = new Inflator();
        inflator.RegisterTo(serviceCollection);
        var provider = serviceCollection.BuildServiceProvider();
        var component = inflator.CreateToComponent(headers, provider);
        Assert.NotNull(component);
    }
}
