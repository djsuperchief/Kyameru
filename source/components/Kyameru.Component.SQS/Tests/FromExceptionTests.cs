using Amazon.SQS;
using Amazon.SQS.Model;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NSubstitute.ExceptionExtensions;

namespace Kyameru.Component.Sqs.Tests;

public class FromExceptionTests
{
    [Fact]
    public async Task ProcessBubblesException()
    {
        var sqsClient = Substitute.For<IAmazonSQS>();
        sqsClient.ReceiveMessageAsync(Arg.Any<ReceiveMessageRequest>(), Arg.Any<CancellationToken>()).Returns<Task<ReceiveMessageResponse>>(
            x =>
            {
                throw new NotImplementedException();
            });
        var from = new SqsFrom(sqsClient);
        from.SetHeaders(new Dictionary<string, string>() { { "Host", "MyQueue" } });
        from.Setup();
        await Assert.ThrowsAsync<NotImplementedException>(async () => await from.StartAsync(default));
        sqsClient.ClearSubstitute();
        sqsClient.ClearReceivedCalls();
    }
}