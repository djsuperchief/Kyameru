using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Component.Rest.Messages;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;

namespace Kyameru.Console.Test;

public class Processor(IKExchange exchange) : IMessageProcessor
{
    
    public event EventHandler<Log> OnLog;
    public async Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
    {
        var messageQueue = routable.Headers.TryGetValue("MessageQueue", string.Empty);
        if (!string.IsNullOrWhiteSpace(messageQueue))
        {
            await exchange.PublishMessageAsync(messageQueue, HttpMessageData.Create(routable), cancellationToken);
        }
    }
}