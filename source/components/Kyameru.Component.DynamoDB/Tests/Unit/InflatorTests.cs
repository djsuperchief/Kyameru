using Amazon.DynamoDBStreams;
using Kyameru.Component.DynamoDB.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Amazon.DynamoDBv2;
using Kyameru.Core.Entities;
using Kyameru.Core.Exceptions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Kyameru.Component.DynamoDB.Tests;

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
    
}