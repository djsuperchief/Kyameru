using System;

namespace Kyameru.Core.Exceptions
{
    /// <summary>
    /// Dependency register exception.
    /// </summary>
    public class DependencyRegisterException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyRegisterException"/> class.
        /// </summary>
        /// <param name="message">Error message.</param>
        public DependencyRegisterException(string message) : base(message)
        {
        }
    }
}