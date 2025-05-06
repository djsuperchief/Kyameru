using System;
using System.Collections.Generic;
using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.Generic;

public class Inflator : IOasis
{
    public IAtomicLink CreateAtomicComponent(Dictionary<string, string> headers)
    {
        throw new NotImplementedException();
    }

    public IFromChainLink CreateFromComponent(Dictionary<string, string> headers, bool isAtomic, IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<IGenericFrom>();
    }

    public IScheduleChainLink CreateScheduleComponent(Dictionary<string, string> headers, bool isAtomic, IServiceProvider serviceProvider)
    {
        throw new NotImplementedException();
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
