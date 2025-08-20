using System;
using System.Collections.Generic;
using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.Generic;

public class Inflator : IOasis
{
    public bool EventsEnabled => true;

    public IFromChainLink CreateFromComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<IGenericFrom>();
    }

    public IScheduleChainLink CreateScheduleComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
    {
        throw new NotImplementedException();
    }

    public IFromEventChainLink CreateFromEvent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<IGenericEventFrom>();
    }

    public IToChainLink CreateToComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<IGenericTo>();
    }

    public IServiceCollection RegisterFrom(IServiceCollection serviceCollection)
    {
        return serviceCollection;
    }

    public IServiceCollection RegisterScheduled(IServiceCollection serviceCollection)
    {
        return serviceCollection;
    }

    public IServiceCollection RegisterTo(IServiceCollection serviceCollection)
    {
        return serviceCollection;
    }
}
