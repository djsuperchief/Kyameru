using System;

namespace Kyameru.Core.Contracts
{
    /// <summary>
    /// Interface for route messaging.
    /// </summary>
    /// <remarks>
    /// Used as part of the internal message bus if components chain link supports it.
    /// </remarks>
    public interface IRouteCommsMessage
    {
        /// <summary>
        /// Gets the unique message id.
        /// </summary>
        Guid MessageId { get; }
        
        /// <summary>
        /// Gets the message data.
        /// </summary>
        object Data { get; }
    }
}