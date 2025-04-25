using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Entities;

namespace Kyameru.Tests.Mocks;

public class ConditionalProcessingPass : IProcessor
{
    public event EventHandler<Log> OnLog;

    public Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
    {
        if (!cancellationToken.IsCancellationRequested)
        {
            routable.SetBody<string>("CondPass");
        }

        OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Information, "Processing"));

        return Task.CompletedTask;
    }
}
