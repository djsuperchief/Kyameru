using System.Security.Cryptography.X509Certificates;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;

namespace Kyameru.Component.Sns;

public class SnsTo : ITo
{
    public event EventHandler<Log> OnLog;

    private readonly string[] requiredHeaders = ["ARN"];
    private readonly IAmazonSimpleNotificationService snsClient;
    private Dictionary<string, string> snsHeaders;

    public SnsTo(IAmazonSimpleNotificationService client)
    {
        snsClient = client;
    }

    public async Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
    {
        try
        {
            Log(LogLevel.Information, Resources.INFO_PROCESSING);
            var message = SnsMessage.FromRoutable(routable, snsHeaders);
            var request = new PublishRequest
            {
                TopicArn = message.Arn,
                Message = message.Message,
                MessageDeduplicationId = routable.Headers.TryGetValue("SNSDeduplicationId", string.Empty),
                MessageGroupId = routable.Headers.TryGetValue("SNSGroupId", string.Empty)
            };

            if (message.Attributes.Count > 0)
            {
                request.MessageAttributes = message.Attributes.ToDictionary(x => x.Key, x => new MessageAttributeValue
                {
                    DataType = "String",
                    StringValue = x.Value
                });
            }
            Log(LogLevel.Information, Resources.INFO_SENDING);
            var response = await snsClient.PublishAsync(request, cancellationToken);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                routable.SetHeader("&SNSMessageId", response.MessageId);
                routable.SetHeader("&SNSSequenceNumber", response.SequenceNumber);
                Log(LogLevel.Information, string.Format(Resources.INFO_SENT, response.MessageId));
            }
            else
            {
                var errorMessage = string.Format(Resources.ERROR_HTTP_RESPONSE, response.HttpStatusCode);
                Log(LogLevel.Information, errorMessage);
                routable.SetInError(new Error("SNS", "To", errorMessage));
            }
        }
        catch (Exception ex)
        {
            routable.SetInError(new Error("SNS", "To", ex.Message));
            Log(LogLevel.Error, string.Format(Resources.ERROR_SENDING, ex.Message), ex);
        }
    }
    public void SetHeaders(Dictionary<string, string> headers)
    {
        var missing = requiredHeaders.Where(x => headers.Keys.All(y => y != x));
        if (missing.Any())
        {
            throw new Core.Exceptions.ComponentException(string.Format(Resources.MISSING_HEADER_EXCEPTION, string.Join(",", missing.ToList())));
        }

        snsHeaders = headers;
    }

    private void Log(LogLevel logLevel, string message, Exception? exception = null)
    {
        if (this.OnLog != null)
        {
            this.OnLog?.Invoke(this, new Core.Entities.Log(logLevel, message, exception));
        }
    }
}
