using Kyameru.Core.Entities;

namespace Kyameru.Core.Contracts
{
    /// <summary>
    /// End processing component.
    /// </summary>
    public interface IToComponent : IComponent
    {
        /// <summary>
        /// Process the incoming request.
        /// </summary>
        /// <param name="item">Message to be processed.</param>
        void Process(Routable item);
    }
}