using Kyameru.Core.Entities;

namespace Kyameru.Component.Sns;

public class SnsTo : ITo
{
    public event EventHandler<Log> OnLog;

    public Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
