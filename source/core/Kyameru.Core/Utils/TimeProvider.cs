using System;
using Kyameru.Core.Contracts;

namespace Kyameru.Core.Utils;

/// <summary>
/// Date time provider.
/// </summary>
/// <remarks>
/// This is more for ensuring that we can test the cron.
/// </remarks>
public abstract class TimeProvider
{
    private static ITimeProvider current = new DefaultTimeProvider();

    public static ITimeProvider Current
    {
        get { return current; }
        set { current = value; }
    }
}

internal class DefaultTimeProvider : ITimeProvider
{
    public DateTime Now => DateTime.Now;

    public DateTime UtcNow => DateTime.UtcNow;
}
