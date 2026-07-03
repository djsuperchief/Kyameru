using Amazon.DynamoDBStreams;
using Kyameru.Component.Dynamodb.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Amazon.DynamoDBv2;
using Kyameru.Core.Entities;
using Kyameru.Core.Exceptions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NotSupportedException = Kyameru.Core.Exceptions.NotSupportedException;

namespace Kyameru.Component.Dynamodb.Tests;

public class InflatorTests
{
    [Fact]
    public void RegisterToSucceeds()
    {
        var serviceCollection = new ServiceCollection();
        var inflator = new Inflator();
        inflator.RegisterTo(serviceCollection);
        Assert.True(serviceCollection.Contains(typeof(ITo), typeof(DynamoDbTo)));
        Assert.True(serviceCollection.Contains(typeof(IAmazonDynamoDB)));
        Assert.True(serviceCollection.Contains(typeof(IDynamoDbUpserter)));
    }

    [Fact]
    public void CreateToComponentSucceeds()
    {
        var serviceCollection = new ServiceCollection();
        var mockDynamoClient = Substitute.For<IAmazonDynamoDB>();
        serviceCollection.AddSingleton<IAmazonDynamoDB>(mockDynamoClient);
        serviceCollection.AddLogging();
        var dynamoDbUri = new RouteAttributes("DynamoDB://test-table");
        var inflator = new Inflator();
        inflator.RegisterTo(serviceCollection);
        var provider = serviceCollection.BuildServiceProvider();
        var component = inflator.CreateToComponent(dynamoDbUri.Headers, provider);

        Assert.NotNull(component);
    }

    [Fact]
    public void ToMissingTableNameThrowsException()
    {
        var serviceCollection = new ServiceCollection();
        var mockDynamoClient = Substitute.For<IAmazonDynamoDB>();
        serviceCollection.AddSingleton<IAmazonDynamoDB>(mockDynamoClient);
        serviceCollection.AddLogging();
        var dynamoDbUri = new RouteAttributes("DynamoDB:///");
        var inflator = new Inflator();
        inflator.RegisterTo(serviceCollection);
        var provider = serviceCollection.BuildServiceProvider();
        Assert.Throws<MissingHeaderException>(() => inflator.CreateToComponent(dynamoDbUri.Headers, provider));
    }
    
    [Fact]
    public void RegisterFromSucceeds()
    {
        var serviceCollection = new ServiceCollection();
        var inflator = new Inflator();
        inflator.RegisterFrom(serviceCollection);
        Assert.True(serviceCollection.Contains(typeof(IFrom), typeof(DynamoDbFrom)));
        Assert.True(serviceCollection.Contains(typeof(IAmazonDynamoDB)));
        Assert.True(serviceCollection.Contains(typeof(IAmazonDynamoDBStreams)));
    }

    [Fact]
    public void CreateFromComponentSucceeds()
    {
        var serviceCollection = new ServiceCollection();
        var mockDynamoClient = Substitute.For<IAmazonDynamoDB>();
        serviceCollection.AddSingleton<IAmazonDynamoDB>(mockDynamoClient);
        serviceCollection.AddSingleton<IAmazonDynamoDBStreams>(Substitute.For<IAmazonDynamoDBStreams>());
        serviceCollection.AddLogging();
        var dynamoDbUri = new RouteAttributes("DynamoDB://test-table");
        var inflator = new Inflator();
        inflator.RegisterFrom(serviceCollection);
        var provider = serviceCollection.BuildServiceProvider();
        var component = inflator.CreateFromComponent(dynamoDbUri.Headers, provider);

        Assert.NotNull(component);
    }
    
    [Fact]
    public void FromMissingTableNameThrowsException()
    {
        var serviceCollection = new ServiceCollection();
        var mockDynamoClient = Substitute.For<IAmazonDynamoDB>();
        serviceCollection.AddSingleton<IAmazonDynamoDB>(mockDynamoClient);
        serviceCollection.AddSingleton<IAmazonDynamoDBStreams>(Substitute.For<IAmazonDynamoDBStreams>());
        serviceCollection.AddLogging();
        var dynamoDbUri = new RouteAttributes("DynamoDB:///");
        var inflator = new Inflator();
        inflator.RegisterFrom(serviceCollection);
        var provider = serviceCollection.BuildServiceProvider();
        Assert.Throws<MissingHeaderException>(() => inflator.CreateFromComponent(dynamoDbUri.Headers, provider));
    }

    [Fact]
    public void RegisterScheduledRaisesActivationException()
    {
        var serviceCollection = new ServiceCollection();
        var mockDynamoClient = Substitute.For<IAmazonDynamoDB>();
        serviceCollection.AddSingleton<IAmazonDynamoDB>(mockDynamoClient);
        serviceCollection.AddLogging();
        var dynamoDbUri = new RouteAttributes("DynamoDB:///");
        var inflator = new Inflator();
        var ex = Assert.Throws<NotSupportedException>(() => inflator.RegisterScheduled(serviceCollection));
        Assert.Equal("DynamoDB component does not support scheduling", ex.Message);
    }
    
    [Fact]
    public void CreateScheduledRaisesActivationException()
    {
        var serviceCollection = new ServiceCollection();
        var mockDynamoClient = Substitute.For<IAmazonDynamoDB>();
        serviceCollection.AddSingleton<IAmazonDynamoDB>(mockDynamoClient);
        serviceCollection.AddLogging();
        var provider = serviceCollection.BuildServiceProvider();
        var inflator = new Inflator();
        var ex = Assert.Throws<NotSupportedException>(() => inflator.CreateScheduleComponent(new Dictionary<string, string>(), provider));
        Assert.Equal("DynamoDB component does not support scheduling", ex.Message);
    }
}