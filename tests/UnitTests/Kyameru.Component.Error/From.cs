using Kyameru.Core;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kyameru.Component.Error
{
    public class From : BaseError, IFromComponent
    {
        public event EventHandler<Routable> OnAction;

        public event EventHandler<Log> OnLog;
        public event AsyncEventHandler<RoutableEventData> OnActionAsync;

        public From(Dictionary<string, string> headers) : base(headers)
        {
        }

        public void Setup()
        {
            this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Information, "Setup"));
        }

        public void Start()
        {
            if (this.WillError())
            {
                throw new NotImplementedException();
            }

            Routable routable = new Routable(this.headers, "Test Data");
            this.OnAction?.Invoke(this, routable);
        }

        public void Stop()
        {
            // nothing to do.
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                if (this.WillError())
                {
                    throw new NotImplementedException();
                }

                Routable routable = new Routable(this.headers, "Test Data");
                await this.OnActionAsync?.Invoke(this, new RoutableEventData(routable, cancellationToken));
            }

        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // do nothing
            await Task.CompletedTask;
        }
    }
}