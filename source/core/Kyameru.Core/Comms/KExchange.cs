using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Contracts;

namespace Kyameru.Core.Comms
{
    /// <summary>
    /// Kyameru Exchange for communication between application and routes.
    /// </summary>
    public class KExchange : IKExchange
    {
        private readonly IKRouter _router;

        /// <summary>
        /// Instantiates a new instance of the <see cref="KExchange"/> class.
        /// </summary>
        /// <param name="router">Message Router.</param>
        public KExchange(IKRouter router)
        {
            _router = router;      
        }
        
        /// <summary>
        /// Publishes a message into the internal event bus.
        /// </summary>
        /// <param name="message">Message to publish.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task PublishMessageAsync(IRouteCommsMessage message, CancellationToken cancellationToken)
        {
            await _router.PublishAsync(message, cancellationToken);
        }
    }
}