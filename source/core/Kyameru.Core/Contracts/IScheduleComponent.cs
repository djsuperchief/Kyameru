using System;
using System.Threading.Tasks;
using Kyameru.Core.Sys;

namespace Kyameru.Core.Contracts
{
    /// <summary>
    /// Schedule component
    /// </summary>
    public interface IScheduleComponent : IComponent
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
        /// Runs the main scheduled task.
        /// </summary>
        /// <returns>Returns a task representing the asynchronous event.</returns>
        Task Run();
    }
}
