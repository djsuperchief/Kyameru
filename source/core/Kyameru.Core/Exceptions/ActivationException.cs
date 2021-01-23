using System;

namespace Kyameru.Core.Exceptions
{
    /// <summary>
    /// Activation Exception
    /// </summary>
    public class ActivationException : Exception
    {
        /// <summary>
        /// Gets the component responsible for exception.
        /// </summary>
        public string Component { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationException"/> class.
        /// </summary>
        /// <param name="message">Error Message.</param>
        /// <param name="innerException">Inner Exception.</param>
        /// <param name="component">Component responsible for error.</param>
        public ActivationException(string message, Exception innerException, string component) : base(message, innerException)
        {
            this.Component = component;
        }
    }
}