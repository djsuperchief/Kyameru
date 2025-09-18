using System;

namespace Kyameru.Core.Exceptions
{
    /// <summary>
    /// Comms exception.
    /// </summary>
    public class CommsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommsException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public CommsException(string message) : base(message)
        {
        }
    }
}