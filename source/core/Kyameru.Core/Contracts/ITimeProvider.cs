using System;

namespace Kyameru.Core.Contracts
{
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
