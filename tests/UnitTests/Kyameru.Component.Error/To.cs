using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kyameru.Component.Error
{
    public class To : BaseError, IToChainLink
    {
        public event EventHandler<Log> OnLog;

        public To(Dictionary<string, string> headers) : base(headers)
        {
        }

        public void Process(Routable item)
        {
            if (this.WillError())
            {
                this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Error, "Error", new Kyameru.Core.Exceptions.ComponentException("Manual Error")));
                throw new Kyameru.Core.Exceptions.ComponentException("Manual Error");
            }
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