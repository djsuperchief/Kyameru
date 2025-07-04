using System;
using System.Collections.Generic;
using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.Generic;

public class Inflator : IOasis
{
    public IFromChainLink CreateFromComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider, Guid id)
    {
        var from = serviceProvider.GetRequiredService<IGenericFrom>();
        from.SetId(id);
        return from;
    }

    public IScheduleChainLink CreateScheduleComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider, Guid id)
    {
        throw new NotImplementedException();
    }

    public IToChainLink CreateToComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider, Guid id)
    {
        var to = serviceProvider.GetRequiredService<IGenericTo>();
        to.SetId(id);
        return to;
    }

    public IServiceCollection RegisterFrom(IServiceCollection serviceCollection, Guid id)
    {
        return serviceCollection;
    }

    public IServiceCollection RegisterScheduled(IServiceCollection serviceCollection, Guid id)
    {
        return serviceCollection;
    }

    public IServiceCollection RegisterTo(IServiceCollection serviceCollection, Guid id)
    {
        return serviceCollection;
    }
}
