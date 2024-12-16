using System;

namespace Kyameru.Core.Contracts
{
    /// <summary>
    /// Time provider
    /// </summary>
    /// <remarks>
    /// This is mostly for unit testing really.
    /// </remarks>
    public interface ITimeProvider
    {
        /// <summary>
        /// Gets the current date time.
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// Gets the current UTC date time.
        /// </summary>
        DateTime UtcNow { get; }
    }
}
