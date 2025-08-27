using System;
using System.Collections.Generic;
using System.Threading.Channels;
using Kyameru.Core.Contracts;

namespace Kyameru.Core.Contracts
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

        /// <summary>
        /// Subscribes to events from the event bus.
        /// </summary>
        /// <param name="bus">Kyameru Router</param>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <returns>Returns an instance of the <see cref="Channel"/> class.</returns>
        ChannelReader<T> SubscribeToEvents<T>(IKRouter bus) where T : IRouteCommsMessage;
        
        /// <summary>
        /// Creates a from component triggered by an event.
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        IFromEventChainLink CreateFromEvent(Dictionary<string, string> headers, IServiceProvider serviceProvider);
    }
}