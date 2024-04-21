using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;

namespace Kyameru.Component.SQS;

public class SqsTo : ITo
{
    private readonly IAmazonSQS sqsClient;
    private Dictionary<string, string> headers;
    private readonly string[] requiredHeaders = new[] { "Host" };
    public event EventHandler<Log>? OnLog;

    public SqsTo(IAmazonSQS awsSqsClient)
    {
        sqsClient = awsSqsClient;
    }
    
    public void Process(Routable routable)
    {
        throw new NotImplementedException();
    }

    public async Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
    {
        var message = new SqsMessage(headers, routable);
        Log(LogLevel.Information, string.Format(Resources.INFORMATION_SEND, headers["Host"]));
        var response = await sqsClient.SendMessageAsync(message.Queue, message.Body, cancellationToken);
        if (string.IsNullOrWhiteSpace(response.MessageId))
        {
            Log(LogLevel.Error, string.Format(Resources.MESSAGE_SENDING_EXCEPTION, headers["Host"], response.HttpStatusCode));
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
    
    private void Log(LogLevel logLevel, string message, Exception exception = null)
    {
        this.OnLog?.Invoke(this, new Core.Entities.Log(logLevel, message, exception));
    }
}