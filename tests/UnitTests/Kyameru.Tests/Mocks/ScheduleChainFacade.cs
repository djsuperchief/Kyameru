using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;

namespace Kyameru.Tests.Mocks;

public class ScheduleChainFacade : Kyameru.Core.Chain.Scheduled
{
    public ScheduleChainFacade(IScheduleChainLink fromComponent, IChain<Routable> next, ILogger logger, string id, bool isAtomicRoute, bool raiseExceptions, Core.Entities.Schedule targetedSchedule) : base(fromComponent, next, logger, id, isAtomicRoute, raiseExceptions, targetedSchedule)
    {
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        await base.ExecuteAsync(cancellationToken);
    }
}
