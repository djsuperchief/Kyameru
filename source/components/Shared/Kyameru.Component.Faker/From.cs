using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Entities;
using Kyameru.Core.Sys;

namespace Kyameru.Component.Faker
{
    public class From : IFakerFrom
    {
        public event EventHandler<Log>? OnLog;
        public event EventHandler<Routable>? OnAction;
        public event AsyncEventHandler<RoutableEventData>? OnActionAsync;

        private readonly Dictionary<string, string> headers = new Dictionary<string, string>();
        public void Setup()
        {
            // not needed.
        }

        public void Start()
        {
            this.OnAction?.Invoke(this, DoProcessing("FAKER FROM"));
        }

        public void Stop()
        {
            // not needed
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var eventData = new RoutableEventData(DoProcessing("FAKE FROM ASYNC"), cancellationToken);
            await this.OnActionAsync?.Invoke(this, eventData);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
        
        private Routable DoProcessing(string call)
        {
            var routable = new Routable(this.headers, "InjectedData");
            routable.SetHeader("&FROM", "CALL");
            this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Information, call));
            return routable;
        }
    }
}