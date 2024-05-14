using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;

namespace Kyameru.Component.Sqs;

public class SqsTo : ITo
{
    private readonly IAmazonSQS sqsClient;
    private Dictionary<string, string> headers = new();
    private readonly string[] requiredHeaders = new[] { "Host" };
    public event EventHandler<Log>? OnLog;

    public SqsTo(IAmazonSQS awsSqsClient)
    {
        sqsClient = awsSqsClient;
    }


    public async Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
    {
        var message = new SqsMessage(headers, routable);
        try
        {
            Log(LogLevel.Information, string.Format(Resources.INFORMATION_SEND, message.Queue));
            var response = await sqsClient.SendMessageAsync(message.ToSendMessageRequest(), cancellationToken);
            if (string.IsNullOrWhiteSpace(response.MessageId))
            {
                Log(LogLevel.Error, string.Format(Resources.MESSAGE_SENDING_EXCEPTION, message.Queue, response.HttpStatusCode));
            }

            routable.SetHeader("&SQSMessageId", response.MessageId);
            Log(LogLevel.Information, string.Format(Resources.INFORMATION_SENT, response.MessageId));
        }
        catch (Exception ex)
        {
            Log(LogLevel.Error, string.Format(Resources.MESSAGE_SENDING_EXCEPTION, message.Queue, ex.Message), ex);
            throw;
        }
    }

    public void SetHeaders(Dictionary<string, string> incomingHeaders)
    {
        headers = incomingHeaders;
        ValidateHeaders(); // validate at set, no point running component if headers are missing.
    }

    private void ValidateHeaders()
    {
        foreach (var header in requiredHeaders)
        {
            if (!headers.ContainsKey(header) || string.IsNullOrWhiteSpace(headers[header]))
            {
                throw new Exceptions.MissingHeaderException(string.Format(Resources.MISSING_HEADER_EXCEPTION, "Host"));
            }
        }
    }

    private void Log(LogLevel logLevel, string message, Exception? exception = null)
    {
        if (this.OnLog != null)
        {
            this.OnLog?.Invoke(this, new Core.Entities.Log(logLevel, message, exception));
        }
    }
}