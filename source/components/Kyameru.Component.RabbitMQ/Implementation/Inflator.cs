using System;
using System.Collections.Generic;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.RabbitMQ
{
    public class Inflator : IOasis
    {
        public IFromChainLink CreateFromComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }

        public IToChainLink CreateToComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }

        public IScheduleChainLink CreateScheduleComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            throw new NotSupportedException(string.Format(Resources.EXCEPTION_CREATIONNOTSUPPORTED, "scheduling"));
        }

        public IServiceCollection RegisterTo(IServiceCollection serviceCollection)
        {
            throw new NotImplementedException();
        }

        public IServiceCollection RegisterFrom(IServiceCollection serviceCollection)
        {
            throw new NotImplementedException();
        }

        public IServiceCollection RegisterScheduled(IServiceCollection serviceCollection)
        {
            throw new NotSupportedException(string.Format(Resources.EXCEPTION_CREATIONNOTSUPPORTED, "scheduling"));
        }

        public void RegisterDependencies(IServiceCollection services, List<ChainLinkDependency> fromDependencies, List<ChainLinkDependency> toDependencies)
        {
            throw new NotImplementedException();
        }
    }
}