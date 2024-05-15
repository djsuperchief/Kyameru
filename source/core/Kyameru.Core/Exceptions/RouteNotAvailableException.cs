using System;

namespace Kyameru.Core
{
    /// <summary>
    /// Exception thrown when a Kyameru route is not implemented.
    /// </summary>
    public class RouteNotAvailableException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RouteNotAvailableException"/> class.
        /// </summary>
        /// <param name="message">Error Message.</param>
        public RouteNotAvailableException(string message) : base(message)
        {

        }

    }
}
