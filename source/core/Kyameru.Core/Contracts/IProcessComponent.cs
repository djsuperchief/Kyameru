using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;

namespace Kyameru
{
    /// <summary>
    /// Intermediary Processing Component
    /// </summary>
    public interface IProcessComponent : IComponent
    {
        /// <summary>
        /// Process the incoming request.
        /// </summary>
        /// <param name="routable">Message to be processed.</param>
        void Process(Routable routable);
    }
}