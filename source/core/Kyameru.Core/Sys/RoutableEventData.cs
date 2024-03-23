using System;
using System.Threading;
using Kyameru.Core.Entities;

namespace Kyameru.Core
{
    /// <summary>
    /// Event data for async events
    /// </summary>
    public class RoutableEventData
    {
        /// <summary>
        /// Gets the cancellation token.
        /// </summary>
        public CancellationToken CancellationToken { get; private set; }

        /// <summary>
        /// Gets the route data.
        /// </summary>
        public Routable Data { get; private set; }

        /// <summary>
        /// Initialises a new instance of the <see cref="RoutableEventData"/> class.
        /// </summary>
        /// <param name="data">Routable data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public RoutableEventData(Routable data, CancellationToken cancellationToken)
        {
            Data = data;
            CancellationToken = cancellationToken;
        }
    }
}
