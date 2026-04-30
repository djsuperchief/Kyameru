using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Kyameru.Component.DynamoDB.Contracts;
using Kyameru.Component.DynamoDB.Exceptions;
using Kyameru.Component.DynamoDB.Tests.Entities;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Kyameru.Component.DynamoDB.Tests;

public class ToTests
{
    [Fact]
    public async Task WhenProcessingNonDynamoRoutableThenThrowsException()
    {
        var dynamoDbMock = Substitute.For<IDynamoDBContext>();
        var mockLogger = Substitute.For<ILogger<DynamoDbUpserter>>();
        var dbUpserter = new DynamoDbUpserter(dynamoDbMock, mockLogger);
        var to = new DynamoDbTo(dbUpserter);

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
        var mockLogger = Substitute.For<ILogger<DynamoDbUpserter>>();
        var dbUpserter = new DynamoDbUpserter(dynamoDbMock, mockLogger);
        var to = new DynamoDbTo(dbUpserter);

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
        var mockLogger = Substitute.For<ILogger<DynamoDbUpserter>>();
        var dbUpserter = new DynamoDbUpserter(dynamoDbMock, mockLogger);
        var to = new DynamoDbTo(dbUpserter);
        var routable = new Routable(new Dictionary<string, string>(), new TestDbRecord());
        
        var exception = await Record.ExceptionAsync(() => to.ProcessAsync(routable, CancellationToken.None));
        Assert.Null(exception);
    }

    [Fact]
    public async Task WhenProcessingIDynamoDbRoutableThenDbContextIsExecuted()
    {
        var dynamoDbMock = Substitute.For<IDynamoDBContext>();
        var mockLogger = Substitute.For<ILogger<DynamoDbUpserter>>();
        var dbUpserter = new DynamoDbUpserter(dynamoDbMock, mockLogger);
        var to = new DynamoDbTo(dbUpserter);
        var routable = new Routable(new Dictionary<string, string>(), new TestDbRecord());
        
        await to.ProcessAsync(routable, CancellationToken.None);
        await dynamoDbMock.Received(1).SaveAsync<IDynamoRecord>(Arg.Any<IDynamoRecord>(), Arg.Any<SaveConfig>(), Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task WhenProcessingListOfIDynamoDbRoutableThenDbContextIsExecuted()
    {
        var dynamoDbMock = Substitute.For<IDynamoDBContext>();
        var mockLogger = Substitute.For<ILogger<DynamoDbUpserter>>();
        var dbUpserter = new DynamoDbUpserter(dynamoDbMock, mockLogger);
        var to = new DynamoDbTo(dbUpserter);
        var testRecords = new List<TestDbRecord>()
        {
            new TestDbRecord()
        };
        
        var routable = new Routable(new Dictionary<string, string>(), testRecords);
        
        await to.ProcessAsync(routable, CancellationToken.None);
        // TODO: Mock out dynamoDB batch writer and assert.
        //await dynamoDbMock.Received(1).(Arg.Any<List<IDynamoRecord>>(), Arg.Any<SaveConfig>(), Arg.Any<CancellationToken>());
    }
}