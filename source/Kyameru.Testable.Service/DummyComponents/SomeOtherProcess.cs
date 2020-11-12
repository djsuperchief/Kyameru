using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;
using System;

namespace Kyameru.Testable.Service.DummyComponents
{
    public class SomeOtherProcess : Kyameru.IProcessComponent
    {
        public event EventHandler<Log> OnLog;

        public void Process(Routable routable)
        {
            this.Log(LogLevel.Information, routable.Headers["Test"]);
        }

        private void Log(LogLevel logLevel, string message, Exception exception = null)
        {
            this.OnLog?.Invoke(this, new Core.Entities.Log(logLevel, message, exception));
        }
    }
}