using Kyameru.Component.DynamoDB.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Amazon.DynamoDBv2;
using Kyameru.Core.Entities;
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
        serviceCollection.AddTransient<ILogger>(Substitute.For<ILogger<DynamoDbUpserter>>());
        var dynamoDbUri = new RouteAttributes("DynamoDB://test-table");
        var inflator = new Inflator();
        inflator.RegisterTo(serviceCollection);
        var provider = serviceCollection.BuildServiceProvider();
        var component = inflator.CreateToComponent(dynamoDbUri.Headers, provider);

        Assert.NotNull(component);
    }
}