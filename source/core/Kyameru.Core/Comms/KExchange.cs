using Kyameru.Core.Contracts;

namespace Kyameru.Core.Comms
{
    /// <summary>
    /// Kyameru Exchange for communication between application and routes.
    /// </summary>
    public class KExchange : IKExchange
    {
        /// <summary>
        /// Publishes a message into the internal event bus.
        /// </summary>
        /// <param name="message">Message to publish.</param>
        public void PublishMessage(IRouteCommsMessage message)
        {
            throw new System.NotImplementedException();
        }
    }
}