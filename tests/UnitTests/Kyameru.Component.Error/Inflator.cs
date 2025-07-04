using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Component.Error
{
    public class Inflator : IOasis
    {
        public IFromChainLink CreateFromComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider, Guid id)
        {
            return new From(headers);
        }

        public IToChainLink CreateToComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider, Guid id)
        {
            return new To(headers);
        }

        public IServiceCollection RegisterTo(IServiceCollection serviceCollection, Guid id)
        {
            return serviceCollection;
        }

        public IServiceCollection RegisterFrom(IServiceCollection serviceCollection, Guid id)
        {
            return serviceCollection;
        }

        public IScheduleChainLink CreateScheduleComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider, Guid id)
        {
            throw new NotSupportedException();
        }

        public IServiceCollection RegisterScheduled(IServiceCollection serviceCollection, Guid id)
        {
            return serviceCollection;
        }
    }
}