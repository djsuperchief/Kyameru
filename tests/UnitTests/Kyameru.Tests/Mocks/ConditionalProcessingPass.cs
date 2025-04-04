using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Entities;

namespace Kyameru.Tests.Mocks;

public class ConditionalProcessingPass : IProcessComponent
{
    public event EventHandler<Log> OnLog;

    public Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
    {
        if (!cancellationToken.IsCancellationRequested)
        {
            routable.SetBody<string>("CondPass");
        }

        return Task.CompletedTask;
    }
}
