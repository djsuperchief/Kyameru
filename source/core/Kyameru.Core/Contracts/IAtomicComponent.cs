using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Entities;

namespace Kyameru.Core.Contracts
{
    /// <summary>
    /// Atomic component executed after the final to.
    /// </summary>
    /// <remarks>
    /// This is still a work in progress, this concept may change in later versions.
    /// </remarks>
    public interface IAtomicComponent : IComponent
    {
        /// <summary>
        /// Process the incoming request.
        /// </summary>
        /// <param name="item">Message to be processed.</param>
        void Process(Routable item);

        /// <summary>
        /// Process the atomic part of the component
        /// </summary>
        /// <param name="item">Message to be processed.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Returns an instance of the <see cref="Task"/> class.</returns>
        Task ProcessAsync(Routable item, CancellationToken cancellationToken);
    }
}
