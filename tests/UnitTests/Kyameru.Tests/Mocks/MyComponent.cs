using Kyameru.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                Process(routable);
            }

            await Task.CompletedTask;
        }
    }
}