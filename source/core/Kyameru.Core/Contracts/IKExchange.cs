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
        void PublishMessage(IRouteCommsMessage message);
    }
}