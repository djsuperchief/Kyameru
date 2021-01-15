using System;

namespace Kyameru.Core.Exceptions
{
    /// <summary>
    /// RouteException
    /// </summary>
    public class RouteUriException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RouteUriException"/> class.
        /// </summary>
        /// <param name="message">Error Message.</param>
        /// <param name="innerException">Inner Exception.</param>
        public RouteUriException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}