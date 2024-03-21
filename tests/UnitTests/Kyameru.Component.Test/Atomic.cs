using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kyameru.Component.Test
{
    public class Atomic : IAtomicComponent
    {
        private readonly Dictionary<string, string> headers;

        public event EventHandler<Log> OnLog;

        public Atomic(Dictionary<string, string> headers)
        {
            this.headers = headers;
        }

        public void Process(Routable item)
        {
            this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Information, "ATOMIC"));
            GlobalCalls.AddCall(item.Headers["TestName"], "ATOMIC");
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