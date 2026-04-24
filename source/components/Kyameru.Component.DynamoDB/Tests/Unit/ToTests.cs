using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Kyameru.Component.DynamoDB.Exceptions;
using Kyameru.Component.DynamoDB.Tests.Entities;
using Kyameru.Core.Entities;
using NSubstitute;

namespace Kyameru.Component.DynamoDB.Tests;

public class ToTests
{
    [Fact]
    public async Task WhenProcessingNonDynamoRoutableThenThrowsException()
    {
        var dynamoDbMock = Substitute.For<IDynamoDBContext>();
        var to = new DynamoDbTo(dynamoDbMock);

        var routable = new Routable(new Dictionary<string, string>(), "hello");

        var exception = await Record.ExceptionAsync(() => to.ProcessAsync(routable, CancellationToken.None));
        
        Assert.NotNull(exception);
        Assert.IsType<InvalidTypeException>(exception);
        Assert.Equal("Routable is not a compatible DynamoDB table.", exception.Message);
    }
    
    [Fact]
    public async Task WhenProcessingEmptyRoutableThenThrowsException()
    {
        var dynamoDbMock = Substitute.For<IDynamoDBContext>();
        var to = new DynamoDbTo(dynamoDbMock);

        var routable = new Routable(new Dictionary<string, string>(), "hello");

        var exception = await Record.ExceptionAsync(() => to.ProcessAsync(null, CancellationToken.None));
        
        Assert.NotNull(exception);
        Assert.IsType<InvalidTypeException>(exception);
        Assert.Equal("Routable is empty.", exception.Message);
    }
    
    [Fact]
    public async Task WhenProcessingIDynamoDbRoutableThenNoException()
    {
        var dynamoDbMock = Substitute.For<IDynamoDBContext>();
        var to = new DynamoDbTo(dynamoDbMock);
        var routable = new Routable(new Dictionary<string, string>(), new TestDbTable());
        
        var exception = await Record.ExceptionAsync(() => to.ProcessAsync(routable, CancellationToken.None));
        Assert.Null(exception);
    }
}