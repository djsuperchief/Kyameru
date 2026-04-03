using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Chain;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;

namespace Kyameru.Tests.Mocks;

internal class TestableFrom : From
{
    public TestableFrom(IFromChainLink fromComponent, IChain<Routable> next, ILogger logger, string id, bool raiseExceptions) : base(fromComponent, next, logger, id, raiseExceptions)
    {
    }
    
    public Task InvokeExecuteAsync(CancellationToken cancellationToken) => base.ExecuteAsync(cancellationToken);
}