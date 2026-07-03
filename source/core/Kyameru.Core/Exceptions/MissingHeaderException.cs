using System;

namespace Kyameru.Core.Exceptions
{
    /// <summary>
    /// Missing header exception.
    /// </summary>
    /// <remarks>
    /// Primarily used in components when setup headers are missing.
    /// </remarks>
    public class MissingHeaderException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingHeaderException"/> exception. 
        /// </summary>
        /// <param name="message">Exception message.</param>
        public MissingHeaderException(string message) :  base(message) { }
    }
}