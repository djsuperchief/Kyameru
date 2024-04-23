using System.Collections.Generic;
using System.Linq;
using Amazon.SQS.Model;
using Kyameru.Core.Entities;

namespace Kyameru.Component.Sqs;

public class SqsMessage
{
    private readonly Headers message;
    private readonly Dictionary<string, string> component;

    public SqsMessage(Dictionary<string, string> componentHeaders, Routable routable)
    {
        component = componentHeaders;
        message = routable.Headers;
        Body = (routable.Body as string)!;
    }

    public string Queue => message.TryGetValue("SQSQueue", component["Host"]);

    public string Body { get; private set; }

    public SendMessageRequest ToSendMessageRequest()
    {
        var response = new SendMessageRequest(Queue, Body);
        foreach (var header in message)
        {
            if (header.Key != "SQSQueue")
            {
                response.MessageAttributes.Add(header.Key, new MessageAttributeValue()
                {
                    DataType = "String",
                    StringValue = header.Value
                });
            }
        }

        return response;
    }
}