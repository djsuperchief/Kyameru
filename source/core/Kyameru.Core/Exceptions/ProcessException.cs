using System;

namespace Kyameru.Core.Exceptions
{
    /// <summary>
    /// Process Exception
    /// </summary>
    public class ProcessException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessException"/> class.
        /// </summary>
        /// <param name="message">Error Message.</param>
        public ProcessException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessException"/> class.
        /// </summary>
        /// <param name="message">Error Message.</param>
        /// <param name="innerException">Inner Exception.</param>
        public ProcessException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}