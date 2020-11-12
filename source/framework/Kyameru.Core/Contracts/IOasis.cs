using System.Collections.Generic;

namespace Kyameru.Core.Contracts
{
    /// <summary>
    /// Component activation interface.
    /// </summary>
    public interface IOasis
    {
        /// <summary>
        /// Creates a from component.
        /// </summary>
        /// <param name="headers">Dictionary of headers to apply.</param>
        /// <returns>Returns an instance of the <see cref="IFromComponent"/> interface.</returns>
        IFromComponent CreateFromComponent(Dictionary<string, string> headers);

        /// <summary>
        /// Creates a to component.
        /// </summary>
        /// <param name="headers">Dictionary of headers to apply.</param>
        /// <returns>Returns an instance of the <see cref="IToComponent"/> instance.</returns>
        IToComponent CreateToComponent(Dictionary<string, string> headers);
    }
}