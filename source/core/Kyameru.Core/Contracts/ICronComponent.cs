using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Sys;

namespace Kyameru.Core.Contracts
{
    /// <summary>
    /// Cron component executed at a set interval.
    /// Entry point (like <see cref="IFromComponent"/>)
    /// </summary>
    public interface ICronComponent : IComponent
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
        /// Execute component
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous Start operation.</returns>
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
