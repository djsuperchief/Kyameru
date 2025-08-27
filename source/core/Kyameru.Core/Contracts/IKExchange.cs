using System.Threading;
using System.Threading.Tasks;

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
        /// <param name="message">Message to publish.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task PublishMessageAsync(IRouteCommsMessage message, CancellationToken cancellationToken);
    }
}