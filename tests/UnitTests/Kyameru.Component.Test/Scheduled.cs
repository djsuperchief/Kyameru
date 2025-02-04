using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Kyameru.Core.Sys;

namespace Kyameru.Component.Test
{
    public class Scheduled : IScheduleComponent
    {
        public event AsyncEventHandler<RoutableEventData> OnActionAsync;
        public event EventHandler<Log> OnLog;

        private int counter = 0;

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            counter++;
            var routable = new Routable(new Dictionary<string, string>()
            {
                {"Counter", counter.ToString()}
            }, Kyameru.Core.Utils.TimeProvider.Current.UtcNow);

            OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Debug, "Schedule Has Executed"));
            await OnActionAsync.Invoke(this, new RoutableEventData(routable, cancellationToken));
        }

        public void Setup()
        {
            // nothing
        }
    }
}
