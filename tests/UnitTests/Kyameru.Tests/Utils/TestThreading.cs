using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kyameru.Tests;

public class TestThreading
{
    internal static Thread GetExecutionThread(Func<CancellationToken, Task> threadStart, int timeOut, out CancellationTokenSource cancellationTokenSource)
    {
        var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeOut));
        cancellationTokenSource = tokenSource;
        return new Thread(async () =>
        {
            await threadStart(tokenSource.Token);
        });
    }
}
