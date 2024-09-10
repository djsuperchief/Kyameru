using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Kyameru.Core.Sys;

namespace Kyameru.Component.Test
{
    public class Cron : ICronComponent
    {
        private int executionCount = 0;
        public event AsyncEventHandler<RoutableEventData> OnActionAsync;
        public event EventHandler<Log> OnLog;

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var executionTime = DateTime.UtcNow;
            var executionDict = new Dictionary<int, DateTime>();
            executionDict.Add(executionCount++, executionTime);
            var routable = new Routable(new System.Collections.Generic.Dictionary<string, string>(),
            executionDict);
            await OnActionAsync.Invoke(this, new RoutableEventData(routable, cancellationToken));
        }

        public void Setup()
        {
            this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Information, "Setup"));
        }
    }
}
