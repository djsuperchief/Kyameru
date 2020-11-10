using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kyameru.Testable.Service.DummyComponents
{
    public class SomeOtherProcess : Kyameru.IProcessComponent
    {
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
            this.Log(LogLevel.Information, routable.Headers["Test"]);
        }

        public void SetError(Routable routable)
        {
            routable.SetInError("SomeOtherProcess");
        }

        private void Log(LogLevel logLevel, string message, Exception exception = null)
        {
            this.OnLog?.Invoke(this, new Core.Entities.Log(logLevel, message, exception));
        }
    }
}