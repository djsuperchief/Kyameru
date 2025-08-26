using System.Threading.Channels;
using Kyameru.Core.Contracts;

namespace Kyameru.Core
{
    /// <summary>
    /// Activator interface for components utilising event driven triggers.
    /// </summary>
    public interface IEventOasis : IOasis
    {
        /// <summary>
        /// Gets a value indicating whether event driven triggers are supported in the component.
        /// </summary>
        bool EventsEnabled { get; }

        Channel<T> SubscribeToEvents<T>(IKRouter bus);
    }
}