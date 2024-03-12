using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Entities;

namespace Kyameru.Core.Contracts
{
    /// <summary>
    /// From Component.
    /// </summary>
    public interface IFromComponent : IComponent
    {
        /// <summary>
        /// Event raised to trigger processing chain.
        /// </summary>
        event EventHandler<Routable> OnAction;

        /// <summary>
        /// Setup the component.
        /// </summary>
        void Setup();

        /// <summary>
        /// Start the core background process.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the component.
        /// </summary>
        void Stop();

        /// <summary>
        /// Start the core background process async.
        /// </summary>
        Task StartAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Stop the component async.
        /// </summary>
        Task StopAsync(CancellationToken cancellationToken);
    }
}