using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Kyameru.Core.Contracts;
using Kyameru.Core.Exceptions;
using Microsoft.Extensions.Primitives;

namespace Kyameru.Core.Comms
{
    /// <summary>
    /// Kyameru message router.
    /// </summary>
    internal class KRouter : IKRouter
    {
        private readonly ConcurrentDictionary<string, Channel<CommsMessage>> _messageQueues =
            new ConcurrentDictionary<string, Channel<CommsMessage>>();
        
        /// <summary>
        /// Creates a subscription for messages of a type.
        /// </summary>
        /// <returns>An instance of <see cref="ChannelReader{CommsMessage}"/>.</returns>
        public ChannelReader<CommsMessage> Subscribe(string identity)
        {
            if (_messageQueues.ContainsKey(identity))
            {
                throw new Core.Exceptions.ComponentException(string.Format(Resources.ERROR_EVENT_IDENTITY_REGISTERED, identity));
            }
            
            var channel = Channel.CreateUnbounded<CommsMessage>();
            _messageQueues.TryAdd(identity, channel);

            return channel.Reader;
        }

        /// <summary>
        /// Publishes a message.
        /// </summary>
        /// <param name="message">Message to publish.</param>
        /// <param name="cancellationToken">Threading cancellation token.</param>
        public async Task PublishAsync(CommsMessage message, CancellationToken cancellationToken)
        {
            if (_messageQueues.TryGetValue(message.RoutingKey, out Channel<CommsMessage> channel))
            {
               await channel.Writer.WriteAsync(message, cancellationToken);
            }
            else
            {
                throw new CommsException(string.Format(Resources.ERROR_SUBSCRIPTION_NOT_FOUND, "test"));
            }
        }
    }
}