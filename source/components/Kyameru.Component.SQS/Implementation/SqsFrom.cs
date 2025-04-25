using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

    public event AsyncEventHandler<RoutableEventData>? OnActionAsync;

    public bool IsPolling => poller.Enabled;

    private readonly IAmazonSQS sqsClient = client;

    private Dictionary<string, string> headers = new();

    private System.Timers.Timer poller = new();

    private string queue = string.Empty;
    private int pollTime;

    public void Setup()
    {
        VerifyHeaders();
        SetQueueLocation();
        poller.Elapsed += Poller_Elapsed;
        poller.Interval = pollTime;
        poller.AutoReset = true;
    }

    private async void Poller_Elapsed(object sender, ElapsedEventArgs e)
    {
        await Process(default);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Log(LogLevel.Information, string.Format(Resources.INFORMATION_SCANSTART, queue));
        poller.Start();
        await Process(cancellationToken); // Perform an initial scan.

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

        pollTime = 10000;
        if (headers.ContainsKey("PollTime"))
        {
            pollTime = int.Parse(headers["PollTime"]) * 1000;
        }
    }

    private async Task Process(CancellationToken cancellationToken)
    {
        Log(LogLevel.Information, Resources.INFORMATION_SCANNING);
        var message = await GetSqsMessage(cancellationToken);
        try
        {

            if (message?.Messages.Count > 0)
            {
                Log(LogLevel.Information, Resources.INFORMATION_MESSAGE_RECEIVED);
                if (await ProcessMessage(message.Messages[0], cancellationToken))
                {
                    await DeleteMessage(message.Messages[0], cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            Log(LogLevel.Error, string.Format(Resources.SQS_QUEUE_SCAN_EXCEPTION, headers["Host"], ex.Message), ex);
            throw;
        }
    }

    private async Task DeleteMessage(Message message, CancellationToken cancellationToken)
    {
        await sqsClient.DeleteMessageAsync(queue, message.ReceiptHandle, cancellationToken);
    }

    private async Task<bool> ProcessMessage(Message message, CancellationToken cancellationToken)
    {
        Log(LogLevel.Information, string.Format(Resources.INFORMATION_PROCESSING_RECEIVED, message.MessageId));
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
            QueueUrl = queue,
            MaxNumberOfMessages = 1,
            WaitTimeSeconds = 2
        }, cancellationToken);
    }

    private void SetQueueLocation()
    {
        if (headers.ContainsKey("Port") ||
            (headers.ContainsKey("Target") && headers["Target"] != "/"))
        {
            if (!headers.TryGetValue("http", out var protocol))
            {
                protocol = "https";
            }
            else
            {
                protocol = bool.Parse(protocol) ? "http" : "https";
            }
            // this is a Queue URL
            var builder = new StringBuilder($"{protocol}://");
            builder.Append(headers["Host"]);
            if (headers.ContainsKey("Port"))
            {
                builder.Append($":{headers["Port"]}");
            }
            builder.Append(headers["Target"]);
            queue = builder.ToString();
        }
        else
        {
            queue = headers["Host"];
        }
    }
}