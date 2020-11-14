using Kyameru.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Tests.Components
{
    public class ProcessingComponent : IProcessComponent
    {
        public event EventHandler<Log> OnLog;

        public void Process(Routable routable)
        {
            this.OnLog(this, new Log(Microsoft.Extensions.Logging.LogLevel.Information, "Hello world"));
        }
    }
}