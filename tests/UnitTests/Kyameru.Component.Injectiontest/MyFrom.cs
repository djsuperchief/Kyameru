using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core;
using Kyameru.Core.Entities;
using Kyameru.Core.Sys;

namespace Kyameru.Component.Injectiontest
{
    public class MyFrom : IMyFrom
    {

        private Dictionary<string, string> headers;

        public event EventHandler<Routable> OnAction;
        public event EventHandler<Log> OnLog;
        public event AsyncEventHandler<RoutableEventData> OnActionAsync;

        public MyFrom()
        {

        }

        public void AddHeaders(Dictionary<string, string> headers)
        {
            this.headers = headers;
        }

        public void Setup()
        {
            this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Information, "Setup"));
        }

        public void Start()
        {
            this.OnAction?.Invoke(this, DoProcessing("FROM"));
        }

        public void Stop()
        {
            this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Information, "Stop"));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var eventData = new RoutableEventData(DoProcessing("FROMASYNC"), cancellationToken);
            await this.OnActionAsync?.Invoke(this, eventData);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Stop();

            await Task.CompletedTask;
        }

        private Routable DoProcessing(string call)
        {
            var routable = new Routable(this.headers, "InjectedData");
            routable.SetHeader("&FROM", "ASYNC");
            GlobalCalls.Calls.Add(call);
            this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Information, call));
            return routable;
        }
    }
}
