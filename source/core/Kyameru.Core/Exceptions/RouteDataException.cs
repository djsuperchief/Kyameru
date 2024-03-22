using System;

namespace Kyameru.Core.Exceptions
{
    /// <summary>
    /// Route data exception raised when inner routing data errors.
    /// </summary>
    public class RouteDataException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RouteDataException"/> class.
        /// </summary>
        /// <param name="message">Error Message.</param>
        /// <param name="innerException">Inner Exception.</param>
        public RouteDataException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteDataException"/> class.
        /// </summary>
        /// <param name="message">Error Message.</param>
        public RouteDataException(string message) : base(message)
        { }
    }
}
