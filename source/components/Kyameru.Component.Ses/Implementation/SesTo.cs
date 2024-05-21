using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Entities;

namespace Kyameru.Component.Ses;

public class SesTo : ITo
{
    public event EventHandler<Log> OnLog;

    public Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
