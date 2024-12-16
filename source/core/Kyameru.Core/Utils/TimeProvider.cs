using System;
using Kyameru.Core.Contracts;

namespace Kyameru.Core.Utils
{
    /// <summary>
    /// Date time provider
    /// </summary>
    /// <remarks>
    /// Primarily for ensuring we can test schedules.
    /// </remarks>
    public abstract class TimeProvider
    {
        private static ITimeProvider current = new DefaultTimeProvider();

        /// <summary>
        /// Current time provider.
        /// </summary>
        public static ITimeProvider Current
        {
            get { return current; }
            set { current = value; }
        }

        /// <summary>
        /// Resets the time provider to default.
        /// </summary>
        public static void Reset() => Current = new DefaultTimeProvider();
    }

    internal class DefaultTimeProvider : ITimeProvider
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
