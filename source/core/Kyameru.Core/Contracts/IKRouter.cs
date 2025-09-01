using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Kyameru.Core.Comms;

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
        /// <returns>An instance of <see cref="ChannelReader{CommsMessage}"/>.</returns>
        ChannelReader<CommsMessage> Subscribe(string identity);

        /// <summary>
        /// Publishes a message.
        /// </summary>
        /// <param name="message">Message to publish.</param>
        /// <param name="cancellationToken">Threading cancellation token.</param>
        /// <exception cref="Exceptions.CommsException"></exception>
        Task PublishAsync(CommsMessage message, CancellationToken cancellationToken);
    }
}