using System.Linq;
using Amazon.SimpleNotificationService;
using Kyameru.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Kyameru.Component.Sns.Tests;

public class InjectionTests
{
    [Fact]
    public void RegisterFromThrowsException()
    {
        var serviceCollection = new ServiceCollection();
        var inflator = new Inflator();
        Assert.Throws<NotImplementedException>(() => inflator.RegisterFrom(serviceCollection));
    }

    [Fact]
    public void RegisterAtomicThrowsException()
    {
        var serviceCollection = new ServiceCollection();
        var inflator = new Inflator();
        Assert.Throws<NotImplementedException>(() => inflator.RegisterFrom(serviceCollection));
    }

    [Fact]
    public void RegisterToSucceeds()
    {
        var serviceCollection = new ServiceCollection();
        var inflator = new Inflator();
        inflator.RegisterTo(serviceCollection);
        Assert.True(serviceCollection.Contains(typeof(ITo), typeof(SnsTo)));
    }

    [Fact]
    public void CreateToComponentSucceeds()
    {
        var inflator = new Inflator();
        var provider = RegisterTo(inflator);
        var component = inflator.CreateToComponent(new Dictionary<string, string>()
        {
            { "ARN", "valid:arn" }
        }, provider);
        Assert.NotNull(component);
    }

    [Fact]
    public void CreateToWithNoHeadersThrowsException()
    {
        var inflator = new Inflator();
        var provider = RegisterTo(inflator);
        Assert.Throws<ComponentException>(() => inflator.CreateToComponent(new Dictionary<string, string>(), provider));
    }

    private ServiceProvider RegisterTo(Inflator inflator)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<IAmazonSimpleNotificationService>(x =>
        {
            return Substitute.For<IAmazonSimpleNotificationService>();
        });
        inflator.RegisterTo(serviceCollection);
        return serviceCollection.BuildServiceProvider();
    }
}
