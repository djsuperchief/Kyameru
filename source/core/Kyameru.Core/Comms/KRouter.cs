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
        private readonly ConcurrentDictionary<Type, List<object>> _channels =
            new ConcurrentDictionary<Type, List<object>>();
        
        /// <summary>
        /// Creates a subscription for messages of a type.
        /// </summary>
        /// <typeparam name="T">Message type to subscribe to.</typeparam>
        /// <returns>An instance of <see cref="ChannelReader{T}"/>.</returns>
        public ChannelReader<T> Subscribe<T>() where T : class, IRouteCommsMessage
        {
            var channel = Channel.CreateUnbounded<T>();
            var channels = _channels.GetOrAdd(typeof(T), _ => new List<object>());
            lock (channels)
            {
                channels.Add(channel);
            }

            return channel.Reader;
        }

        /// <summary>
        /// Publishes a message.
        /// </summary>
        /// <param name="message">Message to publish.</param>
        /// <param name="cancellationToken">Threading cancellation token.</param>
        /// <typeparam name="T">Type of <see cref="IRouteCommsMessage"/>.</typeparam>
        public async Task PublishAsync<T>(T message, CancellationToken cancellationToken) where T : class, IRouteCommsMessage
        {
            if (_channels.TryGetValue(message.GetType(), out List<object> channels))
            {
                List<object> channelsSnapshot;
                lock (channels)
                {
                    channelsSnapshot = channels.ToList();
                }

                foreach (var channel in channelsSnapshot)
                {
                    var broadcastChannel = (Channel<T>)channel;
                    await broadcastChannel.Writer.WriteAsync(message, cancellationToken);
                }
            }
            else
            {
                throw new CommsException(string.Format(Resources.ERROR_SUBSCRIPTION_NOT_FOUND, typeof(T).FullName));
            }
        }
    }
}