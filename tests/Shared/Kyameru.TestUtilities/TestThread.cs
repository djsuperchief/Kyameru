using System;

namespace Kyameru.TestUtilities;

public class TestThread
{
    public CancellationToken CancelToken => _cancelTokenSource.Token;
    private Thread? _executionThread;

    private readonly AutoResetEvent _waitEvent;

    private readonly CancellationTokenSource _cancelTokenSource;

    private readonly int _executionTimeout;

    private const int StandardTimeout = 10;

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

    protected TestThread(int waitTimeout, int threadTimeout)
    {
        _executionTimeout = waitTimeout;
        _cancelTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(threadTimeout));
        _waitEvent = new AutoResetEvent(false);
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
        if (_executionThread == null) throw new NullReferenceException("Thread is not assigned.");
        _executionThread.Start();
    }

    public void StartAndWait()
    {
        if (_executionThread == null) throw new NullReferenceException("Thread is not assigned.");
        _executionThread.Start();
        _waitEvent.WaitOne(TimeSpan.FromSeconds(_executionTimeout));
    }

    public void SetThread(Func<CancellationToken, Task> threadStart)
    {
        if (_executionThread != null) return;
        _executionThread = new Thread(async () =>
        {
            await threadStart(_cancelTokenSource.Token);
        });
    }

    public void Continue()
    {
        _waitEvent.Set();
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

    /// <summary>
    /// Creates a new test thread with no thread entry point.
    /// </summary>
    /// <remarks>
    /// This is so that we can reference the thread in other methods / delegates.
    /// </remarks>
    /// <param name="waitTimeout">Wait handle timeout.</param>
    /// <param name="threadTimeout">Thread timeout.</param>
    /// <returns>Returns an instance of the <see cref="TestThread"/> class.</returns>
    public static TestThread CreateDeferred(int waitTimeout, int threadTimeout = 0)
    {
        threadTimeout = threadTimeout == 0 ? waitTimeout : threadTimeout;
        return new TestThread(waitTimeout, threadTimeout);
    }

    /// <summary>
    /// Creates a new test thread with no thread entry point and standard timeout of 10 seconds.
    /// </summary>
    /// <returns></returns>
    public static TestThread CreateDeferred()
    {
        return new TestThread(StandardTimeout, StandardTimeout);
    }
}
