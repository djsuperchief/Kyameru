using System;
using Kyameru.Core.Entities;

namespace Kyameru.Core.Contracts
{
    /// <summary>
    /// Atomic component executed after the final to.
    /// </summary>
    public interface IAtomicComponent : IProcessComponent
    {
        /// <summary>
        /// Process the incoming request.
        /// </summary>
        /// <param name="item">Message to be processed.</param>
        ///void Process(Routable item);
    }
}
