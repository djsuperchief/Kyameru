using Amazon.DynamoDBStreams;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Kyameru.Component.DynamoDB.Tests.Entities;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Kyameru.Component.DynamoDB.Tests;

public class FromTests
{
    [Fact]
    public async Task TableNameIsPulledFromHeaders_AndLogs()
    {
        var dynamoDbMock = Substitute.For<IAmazonDynamoDB>();
        var mockStreams = Substitute.For<IAmazonDynamoDBStreams>();

        dynamoDbMock.DescribeTableAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).ReturnsForAnyArgs(x =>
        {
            return new DescribeTableResponse()
            {
                Table = new TableDescription()
                {
                    LatestStreamArn = (string)x[0]
                }
            };
        });
        var logMessage = string.Empty;
        var from = new DynamoDbFrom(dynamoDbMock, mockStreams);
        from.SetHeaders(new Dictionary<string, string>()
        {
            { "Host", "testtable"}
        });
        from.OnLog += (sender, log) =>
        {
            logMessage = log.Message;
        };
        
        // Will throw an exception because of the mocking (or lack of).
        var exception = await Record.ExceptionAsync(async () => from.StartAsync(CancellationToken.None));
         
        await Task.Delay(1000, CancellationToken.None);
        await from.StopAsync(CancellationToken.None);
        await dynamoDbMock.Received(1).DescribeTableAsync("testtable", Arg.Any<CancellationToken>());
        Assert.Equal("Processing table: testtable", logMessage);
    }
}