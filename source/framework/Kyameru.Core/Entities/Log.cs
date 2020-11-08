using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Core.Entities
{
    public class Log
    {
        public LogLevel LogLevel { get; private set; }

        public string Message { get; private set; }

        public Exception Error { get; private set; }

        public Log(
            LogLevel logLevel,
            string message,
            Exception error)
        {
            this.LogLevel = logLevel;
            this.Message = message;
            this.Error = error;
        }
    }
}