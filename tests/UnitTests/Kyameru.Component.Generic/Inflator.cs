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
        throw new NotImplementedException();
    }

    public IScheduleChainLink CreateScheduleComponent(Dictionary<string, string> headers, bool isAtomic, IServiceProvider serviceProvider)
    {
        throw new NotImplementedException();
    }

    public IToChainLink CreateToComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
    {
        throw new NotImplementedException();
    }

    public IServiceCollection RegisterFrom(IServiceCollection serviceCollection)
    {
        throw new NotImplementedException();
    }

    public IServiceCollection RegisterScheduled(IServiceCollection serviceCollection)
    {
        throw new NotImplementedException();
    }

    public IServiceCollection RegisterTo(IServiceCollection serviceCollection)
    {
        throw new NotImplementedException();
    }
}
