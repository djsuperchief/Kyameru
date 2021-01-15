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
        /// <param name="isAtomic">Indicates if the route is atomic.</param>
        /// <returns>Returns an instance of the <see cref="IFromComponent"/> interface.</returns>
        IFromComponent CreateFromComponent(Dictionary<string, string> headers, bool isAtomic);

        /// <summary>
        /// Creates a to component.
        /// </summary>
        /// <param name="headers">Dictionary of headers to apply.</param>
        /// <returns>Returns an instance of the <see cref="IToComponent"/> interface.</returns>
        IToComponent CreateToComponent(Dictionary<string, string> headers);

        /// <summary>
        /// Creates an atomic component.
        /// </summary>
        /// <param name="headers">Dictionary of headers to apply.</param>
        /// <returns>Returns an instance of the <see cref="IAtomicComponent"/> interface.</returns>
        IAtomicComponent CreateAtomicComponent(Dictionary<string, string> headers);
    }
}