using System;

namespace Kyameru.TestUtilities;

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

    protected TestThread(
        Func<CancellationToken, Task> threadStart,
        int waitTimeout,
        int threadTimeout
    )
    {
        _executionTimeout = waitTimeout;
        _cancelTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(threadTimeout));
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

    public async Task CancelAsync()
    {
        await _cancelTokenSource.CancelAsync();
    }

    public void Start()
    {
        _executionThread.Start();
    }

    /// <summary>
    /// Creates a new test thread with a standard cancel and wait timeout.
    /// </summary>
    /// <param name="threadStart">Thread to start.</param>
    /// <param name="timeout">Thread timeout.</param>
    /// <returns>Returns an instance of the <see cref="TestThread"/> class.</returns>
    public static TestThread CreateNew(Func<CancellationToken, Task> threadStart, int timeout)
    {
        return new TestThread(threadStart, timeout);
    }

    /// <summary>
    /// Creates a new test thread with a separate timeout for the wait and thread.
    /// </summary>
    /// <param name="threadStart">Thread to start.</param>
    /// <param name="timeout">Thread timeout.</param>
    /// <returns>Returns an instance of the <see cref="TestThread"/> class.</returns>
    public static TestThread CreateNew(Func<CancellationToken, Task> threadStart, int waitTimeout, int threadTimeout)
    {
        return new TestThread(threadStart, waitTimeout, threadTimeout);
    }
}
