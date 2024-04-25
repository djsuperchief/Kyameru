using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Amazon.SQS;
using Amazon.SQS.Model;
using Kyameru.Core.Entities;
using Kyameru.Core.Sys;
using Microsoft.Extensions.Logging;

namespace Kyameru.Component.Sqs;

public class SqsFrom(IAmazonSQS client) : IFrom
{
    public event EventHandler<Log>? OnLog;
    public event EventHandler<Routable>? OnAction;
    public event AsyncEventHandler<RoutableEventData>? OnActionAsync;

    public bool IsPolling => poller.Enabled;

    private readonly IAmazonSQS sqsClient = client;

    private Dictionary<string, string> headers = new();

    private System.Timers.Timer poller = new();
    private int pollTime;

    public void Setup()
    {
        VerifyHeaders();
        poller.Elapsed += Poller_Elapsed;
        poller.AutoReset = true;
    }

    private async void Poller_Elapsed(object sender, ElapsedEventArgs e)
    {
        await Process(default);
    }

    public void Start()
    {
        throw new NotImplementedException();
    }

    public void Stop()
    {
        // Again, sync code will be being deleted.
        throw new NotImplementedException();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Log(LogLevel.Information, string.Format(Resources.INFORMATION_SCANSTART, headers["Host"]));
        poller.Start();
        await Task.CompletedTask;

    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        poller.Stop();
        poller.Elapsed -= Poller_Elapsed;

        await Task.CompletedTask;
    }

    public void SetHeaders(Dictionary<string, string> incomingHeaders)
    {
        headers = incomingHeaders;
    }

    private void VerifyHeaders()
    {
        if (!headers.ContainsKey("Host") || string.IsNullOrWhiteSpace(headers["Host"]))
        {
            throw new Exceptions.MissingHeaderException(string.Format(Resources.MISSING_HEADER_EXCEPTION, "Host"));
        }

        pollTime = 1000;
        if (headers.ContainsKey("PollTime"))
        {
            pollTime = int.Parse(headers["PollTime"]) * 1000;
        }
    }

    private async Task Process(CancellationToken cancellationToken)
    {
        var message = await GetSqsMessage(cancellationToken);
        if (message.Messages.Count > 0)
        {
            if (await ProcessMessage(message.Messages[0], cancellationToken))
            {
                await DeleteMessage(message.Messages[0], cancellationToken);
            }
        }
    }

    private async Task DeleteMessage(Message message, CancellationToken cancellationToken)
    {
        await sqsClient.DeleteMessageAsync(headers["Host"], message.ReceiptHandle, cancellationToken);
    }

    private async Task<bool> ProcessMessage(Message message, CancellationToken cancellationToken)
    {
        var attributes = message.MessageAttributes.Where(x => x.Value.DataType == "String" || x.Value.DataType == null);
        var routable = new Routable(attributes.ToDictionary(x => x.Key, x => x.Value.StringValue), message.Body);
        if (OnActionAsync != null)
        {
            await OnActionAsync.Invoke(this, new RoutableEventData(routable, cancellationToken));
        }

        return true;
    }

    private void Log(LogLevel logLevel, string message, Exception? exception = null)
    {
        if (this.OnLog != null)
        {
            this.OnLog?.Invoke(this, new Core.Entities.Log(logLevel, message, exception));
        }
    }

    private async Task<ReceiveMessageResponse> GetSqsMessage(CancellationToken cancellationToken)
    {
        return await sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest
        {
            QueueUrl = headers["Host"],
            MaxNumberOfMessages = 1,
            WaitTimeSeconds = 2
        }, cancellationToken);
    }
}