using System;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;

namespace Kyameru.Testable.Service.DummyComponents
{
    public class Something : Kyameru.IProcessComponent
    {
        public Something()
        {
        }

        public event EventHandler<Log> OnLog;

        public void LogCritical(string critical)
        {
            throw new NotImplementedException();
        }

        public void LogError(string error)
        {
            throw new NotImplementedException();
        }

        public void LogException(Exception ex)
        {
            throw new NotImplementedException();
        }

        public void LogInformation(string info)
        {
            throw new NotImplementedException();
        }

        public void LogWarning(string warning)
        {
            throw new NotImplementedException();
        }

        public void Process(Routable routable)
        {
            this.Log(LogLevel.Information, routable.Headers["DataType"]);
        }

        private void Log(LogLevel logLevel, string message, Exception exception = null)
        {
            this.OnLog?.Invoke(this, new Core.Entities.Log(logLevel, message, exception));
        }
    }
}