using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Comms;

namespace Kyameru.Core.Contracts
{
    /// <summary>
    /// Kyameru exchange for route comms.
    /// </summary>
    public interface IKExchange
    {
        /// <summary>
        /// Publishes a message into the internal event bus.
        /// </summary>
        /// <param name="routingKey">Routing key for the message.</param>
        /// <param name="message">Message to publish.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task PublishMessageAsync<T>(string routingKey, T message, CancellationToken cancellationToken) where T : class;
    }
}