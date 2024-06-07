using System;
using System.Collections.Generic;
using Amazon.SimpleEmail;
using Kyameru.Core.Contracts;
using Kyameru.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.Ses;

public class Inflator : IOasis
{
    public IAtomicComponent CreateAtomicComponent(Dictionary<string, string> headers)
    {
        throw new RouteNotAvailableException(string.Format(Core.Resources.ERROR_ROUTE_UNAVAILABLE, "ATOMIC", "SES"));
    }

    public IFromComponent CreateFromComponent(Dictionary<string, string> headers, bool isAtomic, IServiceProvider serviceProvider)
    {
        throw new RouteNotAvailableException(string.Format(Core.Resources.ERROR_ROUTE_UNAVAILABLE, "FROM", "SES"));
    }

    public IToComponent CreateToComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
    {
        var component = serviceProvider.GetRequiredService<ITo>();
        component.SetHeaders(headers);
        return component;
    }

    public IServiceCollection RegisterFrom(IServiceCollection serviceCollection)
    {
        throw new RouteNotAvailableException(string.Format(Core.Resources.ERROR_ROUTE_UNAVAILABLE, "FROM", "SES"));
    }

    public IServiceCollection RegisterTo(IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddAWSService<IAmazonSimpleEmailService>();
        serviceCollection.AddTransient<ITo, SesTo>();
        return serviceCollection;
    }
}
