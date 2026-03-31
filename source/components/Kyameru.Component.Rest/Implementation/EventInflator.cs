using System;
using System.Collections.Generic;
using System.Linq;
using Kyameru.Component.Rest.Contracts;
using Kyameru.Component.Rest.Implementation;
using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kyameru.Component.Rest
{
    public class EventInflator : Inflator, IEventOasis
    {
        public IFromEventChainLink CreateFromEvent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            var authId = _fromChainLinkDependencies
                .First(x => x.DependencyType == typeof(IAuthStrategy)).Id;
            var from = serviceProvider.GetRequiredService<IRestFrom>();
            from.SetHeaders(headers);
            from.AddAuthDependencyId(authId);
            return from;
        }
    }
}