using Microsoft.Extensions.Logging;
using System;

namespace Kyameru.Core.Entities
{
    /// <summary>
    /// Standard log entity.
    /// </summary>
    public class Log
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Log"/> class.
        /// </summary>
        /// <param name="logLevel">Log level.</param>
        /// <param name="message">Log message.</param>
        /// <param name="error">Exception class.</param>
        public Log(
            LogLevel logLevel,
            string message,
            Exception error = null)
        {
            this.LogLevel = logLevel;
            this.Message = message;
            this.Error = error;
        }

        /// <summary>
        /// Gets the log level.
        /// </summary>
        public LogLevel LogLevel { get; private set; }

        /// <summary>
        /// Gets the log message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        public Exception Error { get; private set; }
    }
}