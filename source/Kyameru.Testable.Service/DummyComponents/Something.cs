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

        public void Process(Routable routable)
        {
            routable.SetBody<string>(System.Text.Encoding.UTF8.GetString(
                (byte[])routable.Body));
            this.Log(LogLevel.Information, routable.Headers["DataType"]);
        }

        private void Log(LogLevel logLevel, string message, Exception exception = null)
        {
            this.OnLog?.Invoke(this, new Core.Entities.Log(logLevel, message, exception));
        }
    }
}