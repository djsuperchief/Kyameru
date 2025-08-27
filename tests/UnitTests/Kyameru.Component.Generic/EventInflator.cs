using System.Threading.Channels;
using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.Generic;

public class EventInflator : Inflator, IEventOasis
{
    public bool EventsEnabled => true;
    
    public ChannelReader<T> SubscribeToEvents<T>(IKRouter bus) where T : IRouteCommsMessage
    {
        return (ChannelReader<T>)(object)bus.Subscribe<GenericMessage>();
    }

    public IFromEventChainLink CreateFromEvent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<IGenericEventFrom>();
    }
}