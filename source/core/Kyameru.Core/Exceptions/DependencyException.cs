using System;

namespace Kyameru.Core.Exceptions
{
    /// <summary>
    /// Dependency chain link exception.
    /// </summary>
    public class DependencyException : Exception
    {
        /// <summary>
        /// Instantiates a new instance of the <see cref="DependencyException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public DependencyException(string message) : base(message)
        {
        }
    }
}