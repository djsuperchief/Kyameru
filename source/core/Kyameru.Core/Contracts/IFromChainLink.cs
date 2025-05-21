using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Entities;
using Kyameru.Core.Sys;

namespace Kyameru.Core.Contracts
{
    /// <summary>
    /// From Component.
    /// </summary>
    public interface IFromChainLink : IComponent
    {
        /// <summary>
        /// Event raised to trigger processing the chain async.
        /// </summary>
        event AsyncEventHandler<RoutableEventData> OnActionAsync;

        /// <summary>
        /// Setup the component.
        /// </summary>
        void Setup();

        /// <summary>
        /// Starts the kyameru process.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous Start operation.</returns>
        Task StartAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Stops the kyameru process.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous Stop operation.</returns>
        Task StopAsync(CancellationToken cancellationToken);
    }
}