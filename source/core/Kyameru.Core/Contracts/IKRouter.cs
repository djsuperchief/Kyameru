using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Kyameru.Core.Contracts
{
    /// <summary>
    /// Kyameru router interface.
    /// </summary>
    public interface IKRouter
    {
        /// <summary>
        /// Creates a subscription for messages of a type.
        /// </summary>
        /// <typeparam name="T">Message type to subscribe to.</typeparam>
        /// <returns>An instance of <see cref="ChannelReader{T}"/>.</returns>
        ChannelReader<T> Subscribe<T>();

        /// <summary>
        /// Publishes a message.
        /// </summary>
        /// <param name="message">Message to publish.</param>
        /// <param name="cancellationToken">Threading cancellation token.</param>
        /// <exception cref="Exceptions.CommsException"></exception>
        Task Publish<T>(T message, CancellationToken cancellationToken) where T : class, IRouteCommsMessage;
    }
}