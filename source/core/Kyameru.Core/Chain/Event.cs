using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Kyameru.Core.Comms;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Kyameru.Core.Extensions;
using Kyameru.Core.Sys;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kyameru.Core.Chain
{
    internal class Event : BackgroundService
    {
        private readonly IFromEventChainLink _from;
        private readonly IChain<Routable> _next;
        private readonly ILogger _logger;
        private readonly string _identity;
        private readonly bool _raiseExceptions;
        private readonly ChannelReader<CommsMessage> _messageQueue;

        public Event(IFromEventChainLink fromChainLink, IChain<Routable> next, ILogger logger, string id, ChannelReader<CommsMessage> msgQueue, bool raiseExceptions)
        {
            _from =  fromChainLink;
            _next = next;
            _logger = logger;
            _identity = id;
            _raiseExceptions = raiseExceptions;
            _messageQueue = msgQueue;
            _from.OnActionAsync += From_OnActionAsync;
            _from.OnLog += From_OnLog;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                await foreach (var message in _messageQueue.ReadAllAsync(stoppingToken))
                {
                    try
                    {
                        From_OnLog(this, new Log(LogLevel.Information, string.Format(Resources.INFO_PROCESSING_MESSAGE, message.Id)));
                        await _from.ProcessAsync(message, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        From_OnLog(this, new Log(LogLevel.Error, ex.Message, ex));
                        if (_raiseExceptions)
                        {
                            throw;
                        }
                    }
                }
            } while (!stoppingToken.IsCancellationRequested);
            _logger.LogDebug("Event chain link stopping.");
        }
        
        private async Task From_OnActionAsync(object sender, RoutableEventData e)
        {
            await _next?.HandleAsync(e.Data, e.CancellationToken);
        }
        
        private void From_OnLog(object sender, Log e)
        {
            if (e.Error == null)
            {
                _logger.LogInformation(_identity, e.Message, e.LogLevel);
            }
            else
            {
                _logger.LogError(_identity, e.Message, e.Error);
            }
        }
    }
}