using Amazon.SQS;
using Kyameru.Core;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Kyameru.Component.Sqs.Tests;

public class InjectionTests
{
    [Fact]
    public void RegisterFromSucceeds()
    {
        var serviceCollection = new ServiceCollection();
        var inflator = new Inflator();
        inflator.RegisterFrom(serviceCollection);
        Assert.True(serviceCollection.Contains(typeof(IFrom), typeof(SqsFrom)));
    }

    [Fact]
    public void RegisterToSucceeds()
    {
        var serviceCollection = new ServiceCollection();
        var inflator = new Inflator();
        inflator.RegisterTo(serviceCollection);
        Assert.True(serviceCollection.Contains(typeof(ITo), typeof(SqsTo)));
    }

    [Fact]
    public void CreateAtomicThrowsNotImplemented()
    {
        var inflator = new Inflator();
        Assert.Throws<RouteNotAvailableException>(() => inflator.CreateAtomicComponent(new Dictionary<string, string>()));
    }

    [Fact]
    public void CanCreateToComponent()
    {
        var serviceCollection = new ServiceCollection();
        var sqsClient = Substitute.For<IAmazonSQS>();
        serviceCollection.AddTransient<IAmazonSQS>(x => Substitute.For<IAmazonSQS>());
        var inflator = new Inflator();
        var headers = new Dictionary<string, string>()
        {
            { "Host", "MyQueueArn" }
        };
        inflator.RegisterTo(serviceCollection);
        var provider = serviceCollection.BuildServiceProvider();
        var component = inflator.CreateToComponent(headers, provider);
        Assert.NotNull(component);
    }

    [Fact]
    public void CanCreateFromComponent()
    {
        var serviceCollection = new ServiceCollection();
        var sqsClient = Substitute.For<IAmazonSQS>();
        serviceCollection.AddTransient<IAmazonSQS>(x => Substitute.For<IAmazonSQS>());
        var inflator = new Inflator();
        inflator.RegisterFrom(serviceCollection);
        var provider = serviceCollection.BuildServiceProvider();
        var component = inflator.CreateFromComponent(new Dictionary<string, string>(), false, provider);
        Assert.NotNull(component);
    }
}