using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kyameru.Tests;

public class TestThread
{
    private readonly Thread _executionThread;

    private readonly AutoResetEvent _waitEvent;

    private readonly CancellationTokenSource _cancelTokenSource;

    private readonly int _executionTimeout;

    protected TestThread(
        Func<CancellationToken, Task> threadStart,
        int timeOut
    )
    {
        _executionTimeout = timeOut;
        _cancelTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(_executionTimeout));
        _waitEvent = new AutoResetEvent(false);
        _executionThread = new Thread(async () =>
        {
            await threadStart(_cancelTokenSource.Token);
        });
    }

    public void WaitForExecution()
    {
        _waitEvent.WaitOne(TimeSpan.FromSeconds(_executionTimeout));
    }

    public async Task Cancel()
    {
        await _cancelTokenSource.CancelAsync();
    }

    public void Start()
    {
        _executionThread.Start();
    }

    public static TestThread CreateNew(Func<CancellationToken, Task> threadStart, int timeout)
    {
        return new TestThread(threadStart, timeout);
    }
}
