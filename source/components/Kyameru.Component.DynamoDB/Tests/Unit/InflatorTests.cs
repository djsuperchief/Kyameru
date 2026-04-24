using Kyameru.Component.DynamoDB.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Amazon.DynamoDBv2;

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
    }
}