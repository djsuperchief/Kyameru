using Kyameru.Core.Entities;

namespace Kyameru.Core.Contracts
{
    /// <summary>
    /// End processing component.
    /// </summary>
    public interface IToComponent : IProcessComponent
    {
        /// <summary>
        /// Process the incoming request.
        /// </summary>
        /// <param name="item">Message to be processed.</param>
        /// void Process(Routable item);

        // To component moved to inherit from process component but should move back
        // if we extend the process component outside the bounds of the to
    }
}