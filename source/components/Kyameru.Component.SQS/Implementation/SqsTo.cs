using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Kyameru.Core.Entities;

namespace Kyameru.Component.SQS;

public class SqsTo : ITo
{
    private readonly IAmazonSQS sqsClient;
    private Dictionary<string, string> headers;
    public event EventHandler<Log>? OnLog;

    public SqsTo(IAmazonSQS awsSqsClient)
    {
        sqsClient = awsSqsClient;
    }
    
    public void Process(Routable routable)
    {
        throw new NotImplementedException();
    }

    public Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public void SetHeaders(Dictionary<string, string> incomingHeaders)
    {
        headers = incomingHeaders;
    }
}