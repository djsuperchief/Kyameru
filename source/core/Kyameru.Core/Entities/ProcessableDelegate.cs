using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Entities;

namespace Kyameru.Core.Entities
{
    internal class ProcessableDelegate : IProcessComponent
    {
        public event EventHandler<Log> OnLog;

        private Action<Routable> processAction;

        public ProcessableDelegate(Action<Routable> processor)
        {
            processAction = processor;
        }

        public void Process(Routable routable)
        {
            OnLog?.Invoke(this, new Entities.Log(Microsoft.Extensions.Logging.LogLevel.Information, "Running process delegate"));
            processAction.Invoke(routable);
        }

        public async Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
        {
            if(!cancellationToken.IsCancellationRequested)
            {
                OnLog?.Invoke(this, new Entities.Log(Microsoft.Extensions.Logging.LogLevel.Information, "Running process delegate"));
                processAction.Invoke(routable);
            }

            await Task.CompletedTask;
        }
    }
}
