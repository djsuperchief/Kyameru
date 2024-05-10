using System.Security.Cryptography.X509Certificates;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Kyameru.Core.Entities;

namespace Kyameru.Component.Sns;

public class SnsTo(IAmazonSimpleNotificationService client) : ITo
{
    public event EventHandler<Log> OnLog;

    private readonly string[] requiredHeaders = ["ARN"];
    private readonly IAmazonSimpleNotificationService snsClient = client;
    private Dictionary<string, string> snsHeaders;

    public async Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
    {
        try
        {
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

            var response = await client.PublishAsync(request, cancellationToken);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                routable.SetHeader("SNSMessageId", response.MessageId);
                routable.SetHeader("SNSSequenceNumber", response.SequenceNumber);
            }
        }
        catch (Exception ex)
        {
            routable.SetInError(new Error("SNS", "To", ex.Message));
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
}
