using Kyameru.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Tests.Mocks
{
    public class MyComponent : IMyComponent
    {
        public event EventHandler<Log> OnLog;

        public void Process(Routable routable)
        {
            this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Information, "Setting header"));
            routable.SetHeader("ComponentRan", "Yes");
        }
    }
}