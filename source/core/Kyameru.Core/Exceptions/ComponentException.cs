using System;

namespace Kyameru.Core.Exceptions
{
    /// <summary>
    /// Component Exception
    /// </summary>
    public class ComponentException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentException"/> class.
        /// </summary>
        /// <param name="message">Error Message.</param>
        public ComponentException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentException"/> class.
        /// </summary>
        /// <param name="message">Error Message.</param>
        /// <param name="innerException">Inner Exception.</param>
        public ComponentException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}