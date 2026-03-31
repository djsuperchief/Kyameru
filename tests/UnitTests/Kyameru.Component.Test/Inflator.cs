using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kyameru.Core.Entities;

namespace Kyameru.Component.Test
{
    public class Inflator : IOasis
    {
        public bool EventsEnabled => false;

        public IFromChainLink CreateFromComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            return new From(headers);
        }

        public IToChainLink CreateToComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            return new To(headers);
        }

        public IFromEventChainLink CreateFromEvent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }

        public IServiceCollection RegisterTo(IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }

        public IServiceCollection RegisterFrom(IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }

        public IScheduleChainLink CreateScheduleComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            return new Scheduled();
        }

        public IServiceCollection RegisterScheduled(IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }

        public void RegisterDependencies(IServiceCollection services, List<ChainLinkDependency> fromDependencies, List<ChainLinkDependency> toDependencies)
        {
            
        }
    }
}