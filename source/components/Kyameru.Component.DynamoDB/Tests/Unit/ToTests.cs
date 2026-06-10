using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
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
        var dynamoDbMock = Substitute.For<IAmazonDynamoDB>();
        var mockLogger = Substitute.For<ILogger<DynamoDbUpserter>>();
        var dbUpserter = new DynamoDbUpserter(dynamoDbMock, mockLogger);
        var to = new DynamoDbTo(dbUpserter);
    
        var routable = new Routable(new Dictionary<string, string>(), "hello");
    
        var exception = await Record.ExceptionAsync(() => to.ProcessAsync(routable, CancellationToken.None));
        
        Assert.NotNull(exception);
        Assert.IsType<InvalidTypeException>(exception);
        Assert.Equal("Routable is not a compatible DynamoDB table record", exception.Message);
    }
    
    [Fact]
    public async Task WhenProcessingEmptyRoutableThenThrowsException()
    {
        var dynamoDbMock = Substitute.For<IAmazonDynamoDB>();
        var mockLogger = Substitute.For<ILogger<DynamoDbUpserter>>();
        var dbUpserter = new DynamoDbUpserter(dynamoDbMock, mockLogger);
        var to = new DynamoDbTo(dbUpserter);
    
        var routable = new Routable(new Dictionary<string, string>(), "hello");
    
        var exception = await Record.ExceptionAsync(() => to.ProcessAsync(null, CancellationToken.None));
        
        Assert.NotNull(exception);
        Assert.IsType<InvalidTypeException>(exception);
        Assert.Equal("Routable is empty", exception.Message);
    }
    
    [Fact]
    public async Task WhenProcessingIDynamoDbRoutableThenNoException()
    {
        var dynamoDbMock = Substitute.For<IAmazonDynamoDB>();
        var mockLogger = Substitute.For<ILogger<DynamoDbUpserter>>();
        var dbUpserter = new DynamoDbUpserter(dynamoDbMock, mockLogger);
        var to = new DynamoDbTo(dbUpserter);
        to.SetHeaders(new Dictionary<string, string>()
        {
            { "Host", "testtable"}
        });
        var routable = new Routable(new Dictionary<string, string>(), new TestDbRecord());
        
        var exception = await Record.ExceptionAsync(() => to.ProcessAsync(routable, CancellationToken.None));
        Assert.Null(exception);
    }
    
    [Fact]
    public async Task WhenProcessingIDynamoDbRoutableThenDbContextIsExecuted()
    {
        var dynamoDbMock = Substitute.For<IAmazonDynamoDB>();
        var mockLogger = Substitute.For<ILogger<DynamoDbUpserter>>();
        var dbUpserter = new DynamoDbUpserter(dynamoDbMock, mockLogger);
        var to = new DynamoDbTo(dbUpserter);
        to.SetHeaders(new Dictionary<string, string>()
        {
            { "Host", "testtable"}
        });
        var routable = new Routable(new Dictionary<string, string>(), new TestDbRecord());
        
        await to.ProcessAsync(routable, CancellationToken.None);
        await dynamoDbMock.Received(1).PutItemAsync(Arg.Any<PutItemRequest>(), Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task WhenProcessingIDynamoDbRoutableAndTablenameMissingThenExceptionIsThrown()
    {
        var dynamoDbMock = Substitute.For<IAmazonDynamoDB>();
        var mockLogger = Substitute.For<ILogger<DynamoDbUpserter>>();
        var dbUpserter = new DynamoDbUpserter(dynamoDbMock, mockLogger);
        var to = new DynamoDbTo(dbUpserter);
        var routable = new Routable(new Dictionary<string, string>() { { "DynamoDbOverrideTable", string.Empty }}, new TestDbRecord());
        
        var exception = await Record.ExceptionAsync(() => to.ProcessAsync(routable, CancellationToken.None));
        
        Assert.NotNull(exception);
        Assert.IsType<Exceptions.MissingDynamoTable>(exception);
        Assert.Equal("Missing DynamoDB table name", exception.Message);
    }
    
    [Fact]
    public async Task WhenProcessingListOfIDynamoDbRoutableThenDbContextIsExecuted()
    {
        var dynamoDbMock = Substitute.For<IAmazonDynamoDB>();
        var mockLogger = Substitute.For<ILogger<DynamoDbUpserter>>();
        var dbUpserter = new DynamoDbUpserter(dynamoDbMock, mockLogger);
        var to = new DynamoDbTo(dbUpserter);
        to.SetHeaders(new Dictionary<string, string>()
        {
            { "Host", "testtable"}
        });
        var testRecords = new List<TestDbRecord>()
        {
            new()
        };
        
        var routable = new Routable(new Dictionary<string, string>(), testRecords);
        
        await to.ProcessAsync(routable, CancellationToken.None);
    
        dynamoDbMock.Received(1).BatchWriteItemAsync(Arg.Any<BatchWriteItemRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task TableNameIsPulledFromHeaders()
    {
        var dynamoDbMock = Substitute.For<IAmazonDynamoDB>();
        var mockLogger = Substitute.For<ILogger<DynamoDbUpserter>>();
        var dbUpserter = new DynamoDbUpserter(dynamoDbMock, mockLogger);
        var to = new DynamoDbTo(dbUpserter);
        to.SetHeaders(new Dictionary<string, string>()
        {
            { "Host", "testtable"}
        });
        
        var routable = new Routable(new Dictionary<string, string>(), new TestDbRecord());
        
        await to.ProcessAsync(routable, CancellationToken.None);
        await dynamoDbMock.Received(1).PutItemAsync(Arg.Any<PutItemRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessingLogsAreOutput()
    {
        var dynamoDbMock = Substitute.For<IAmazonDynamoDB>();
        var mockLogger = Substitute.For<ILogger<DynamoDbUpserter>>();
        var dbUpserter = new DynamoDbUpserter(dynamoDbMock, mockLogger);
        
        var to = new DynamoDbTo(dbUpserter);
        to.SetHeaders(new Dictionary<string, string>()
        {
            { "Host", "testtable"}
        });
        var routable = new Routable(new Dictionary<string, string>(), new TestDbRecord());
        await to.ProcessAsync(routable, CancellationToken.None);
        
        var logMessages = new List<string>() { "Processing single record to DynamoDB", "Generating attribute map", "Processing complete" };
        foreach (var logMessage in logMessages)
        {
            mockLogger.Received().Log(Arg.Any<LogLevel>(),
                Arg.Any<EventId>(),
                Arg.Is<object>(v => v.ToString().Contains(logMessage)),
                null,
                Arg.Any<Func<object, Exception, string>>());
        }
    }
}