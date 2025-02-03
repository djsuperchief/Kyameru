using Kyameru.Core;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Sys;

namespace Kyameru.Component.Test
{
    public class From : IFromComponent
    {
        public event EventHandler<Log> OnLog;
        public event AsyncEventHandler<RoutableEventData> OnActionAsync;

        private readonly Dictionary<string, string> headers;

        public From(Dictionary<string, string> headers)
        {
            this.headers = headers;
        }

        public void Setup()
        {
            this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Information, "Setup"));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Routable routable = new Routable(this.headers, "TestData");
            GlobalCalls.AddCall(routable.Headers["TestName"], "FROM");
            this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Information, "FROM"));
            await this.OnActionAsync?.Invoke(this, new RoutableEventData(routable, cancellationToken));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Information, "StopAsync"));
            await Task.CompletedTask;
        }
    }
}