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
        private readonly Func<Routable, Task> processFunc;

        public ProcessableDelegate(Action<Routable> processor)
        {
            processAction = processor;
        }

        public ProcessableDelegate(Func<Routable, Task> processor)
        {
            processFunc = processor;
        }
        public async Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                OnLog?.Invoke(this, new Entities.Log(Microsoft.Extensions.Logging.LogLevel.Information, "Running process delegate"));
                if (processAction != null)
                {
                    processAction.Invoke(routable);
                }
                else
                {
                    await processFunc.Invoke(routable);
                }
            }

            await Task.CompletedTask;
        }
    }
}
