using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon.SQS.Model;
using Kyameru.Core.Entities;

namespace Kyameru.Component.Sqs;

public class SqsMessage
{


    public SqsMessage(Dictionary<string, string> componentHeaders, Routable routable)
    {
        component = componentHeaders;
        message = routable.Headers;
        Body = (routable.Body as string)!;
    }

    public string Queue { get; private set; }

    public string Body { get; private set; }

    private readonly Headers message;
    private readonly Dictionary<string, string> component;

    private const string queueHeader = "SQSQueue";

    public SendMessageRequest ToSendMessageRequest()
    {
        SetQueueLocation();
        var response = new SendMessageRequest(Queue, Body);
        foreach (var header in message)
        {
            if (header.Key != queueHeader && !string.IsNullOrWhiteSpace(header.Value))
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

    private void SetQueueLocation()
    {
        var intermediate = message.TryGetValue("SQSQueue", string.Empty);
        if (!string.IsNullOrWhiteSpace(intermediate))
        {
            GetMessageQueueLocation(intermediate);
            return;
        }

        if (component.ContainsKey("Port") ||
            (component.ContainsKey("Target") && component["Target"] != "/"))
        {
            // this is a Queue URL
            var builder = new StringBuilder($"{GetProtocol()}://");
            builder.Append(component["Host"]);
            if (component.ContainsKey("Port"))
            {
                builder.Append($":{component["Port"]}");
            }
            builder.Append(component["Target"]);
            Queue = builder.ToString();
        }
        else
        {
            Queue = component["Host"];
        }
    }

    private void GetMessageQueueLocation(string intermediate)
    {
        // if the queue contains a whole queue url (including http(s)) then just take it as given.
        if ((intermediate.Contains(":") || intermediate.Contains("/")) && !intermediate.Contains("http"))
        {
            Queue = $"{GetProtocol()}://{intermediate}";
        }
        else
        {
            Queue = intermediate;
        }
    }

    private string GetProtocol()
    {
        if (!component.TryGetValue("http", out var protocol))
        {
            protocol = "https";
        }
        else
        {
            protocol = bool.Parse(protocol) ? "http" : "https";
        }

        return protocol;
    }
}