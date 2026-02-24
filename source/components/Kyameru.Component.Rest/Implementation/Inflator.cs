using System;
using System.Collections.Generic;
using System.Linq;
using Kyameru.Component.Rest.Contracts;
using Kyameru.Component.Rest.Implementation;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Kyameru.Core.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kyameru.Component.Rest
{
    public class Inflator : IOasis
    {
        protected List<Core.Entities.ChainLinkDependency> _fromChainLinkDependencies;
        
        protected List<Core.Entities.ChainLinkDependency> _toChainLinkDependencies;
        
        public IFromChainLink CreateFromComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }

        public IToChainLink CreateToComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            var authId = _toChainLinkDependencies
                .First(x => x.DependencyType == typeof(IAuthStrategy)).Id;
            var toChain = serviceProvider.GetRequiredService<IRestTo>();
            toChain.SetHeaders(headers);
            toChain.AddAuthDependencyId(authId);
            return toChain;
        }

        public IScheduleChainLink CreateScheduleComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }

        public IServiceCollection RegisterTo(IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddTransient<IHttpContentFactory, HttpContentFactory>();
            serviceCollection.TryAddTransient<IRestTo, RestTo>();
            return serviceCollection;
        }

        public IServiceCollection RegisterFrom(IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddTransient<IHttpContentFactory, HttpContentFactory>();
            serviceCollection.TryAddTransient<IRestFrom, RestFrom>();
            return serviceCollection;
        }

        public IServiceCollection RegisterScheduled(IServiceCollection serviceCollection)
        {
            throw new NotImplementedException();
        }

        public void RegisterDependencies(IServiceCollection services, List<ChainLinkDependency>? fromDependencies, List<ChainLinkDependency>? toDependencies)
        {
            if (fromDependencies != null)
            {
                _fromChainLinkDependencies = fromDependencies;
            }

            if (toDependencies != null)
            {
                _toChainLinkDependencies = toDependencies;
            }

            RegisterNoAuth(services);
        }

        protected void RegisterNoAuth(IServiceCollection serviceCollection)
        {
            var auth = new NoAuth();
            _fromChainLinkDependencies ??= new List<ChainLinkDependency>();
            _toChainLinkDependencies ??= new List<ChainLinkDependency>();
            if (_fromChainLinkDependencies.All(x => x.DependencyType != typeof(IAuthStrategy)))
            {
                _fromChainLinkDependencies.Add(serviceCollection.RegisterKyameruDependency<IAuthStrategy>(ChainLinkDependencyType.From,() => auth));
            }
            
            if (_toChainLinkDependencies.All(x => x.DependencyType != typeof(IAuthStrategy)))
            {
                _toChainLinkDependencies.Add(serviceCollection.RegisterKyameruDependency<IAuthStrategy>(ChainLinkDependencyType.To,() => auth));
            }
        }
    }
}