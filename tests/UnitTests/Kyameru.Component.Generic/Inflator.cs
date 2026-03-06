using System;
using System.Collections.Generic;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.Generic;

public class Inflator : IOasis
{


    public IFromChainLink CreateFromComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<IGenericFrom>();
    }

    public IScheduleChainLink CreateScheduleComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
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

    public void RegisterDependencies(IServiceCollection services, List<ChainLinkDependency> fromDependencies, List<ChainLinkDependency> toDependencies)
    {
        
    }

    public IServiceCollection RegisterTo(IServiceCollection serviceCollection)
    {
        return serviceCollection;
    }
}
