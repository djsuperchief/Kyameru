using System;
namespace Kyameru.Core.Exceptions
{
    /// <summary>
    /// Kyameru Core Exception
    /// </summary>
    public class CoreException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreException"/> class.
        /// </summary>
        /// <param name="message">Error Message.</param>
        public CoreException(string message) : base(message)
        {
        }
    }
}
