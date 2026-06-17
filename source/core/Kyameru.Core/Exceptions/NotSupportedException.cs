using System;

namespace Kyameru.Core.Exceptions
{
    /// <summary>
    /// Exception representing a particular function not being supported (I.E scheduled).
    /// </summary>
    public class NotSupportedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotSupportedException"/> exception. 
        /// </summary>
        /// <param name="message">Exception message.</param>
        public NotSupportedException(string message) : base(message)
        {
        }
    }
}