using System.Threading.Channels;
using Kyameru.Core.Comms;
using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.Generic;

public class EventInflator : Inflator, IEventOasis
{
    public bool EventsEnabled => true;

    public IFromEventChainLink CreateFromEvent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<IGenericEventFrom>();
    }
}