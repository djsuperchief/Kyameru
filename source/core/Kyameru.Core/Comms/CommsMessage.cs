using System;

namespace Kyameru.Core.Comms
{
    /// <summary>
    /// Kyameru message for message bus.
    /// </summary>
    public class CommsMessage
    {
        /// <summary>
        /// Gets the id for the message.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the routing key for the message.
        /// </summary>
        public string RoutingKey { get; private set; }
        
        /// <summary>
        /// Gets the data in the message.
        /// </summary>
        public object Data { get; private set; }

        private CommsMessage()
        {
            this.Id = Guid.NewGuid();
        }

        /// <summary>
        /// Creates a new event bus message.
        /// </summary>
        /// <param name="data">Data for message.</param>
        /// <param name="routingKey">Routing key for the message</param>
        /// <returns>Returns an instance of the <see cref="CommsMessage"/></returns>
        public static CommsMessage Create<T>(string routingKey, T data)
        {
            return new CommsMessage()
            {
                RoutingKey = routingKey,
                Data = data
            };
        }
    }
}