﻿using Amazon.S3;
using Kyameru.Core.Exceptions;
using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kyameru.Component.S3;

public class Inflator : IOasis
{
    public IFromChainLink CreateFromComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
    {
        throw new RouteNotAvailableException(string.Format(Core.Resources.ERROR_ROUTE_UNAVAILABLE, "FROM", "S3"));
    }

    public IScheduleChainLink CreateScheduleComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
    {
        throw new NotImplementedException();
    }

    public IToChainLink CreateToComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
    {
        var component = serviceProvider.GetRequiredService<ITo>();
        component.SetHeaders(headers);
        return component;
    }

    public IServiceCollection RegisterFrom(IServiceCollection serviceCollection)
    {
        throw new RouteNotAvailableException(string.Format(Core.Resources.ERROR_ROUTE_UNAVAILABLE, "FROM", "S3"));
    }

    public IServiceCollection RegisterScheduled(IServiceCollection serviceCollection)
    {
        throw new NotImplementedException();
    }

    public IServiceCollection RegisterTo(IServiceCollection serviceCollection)
    {
        // Only add S3 if it hasn't been added by the host
        serviceCollection.TryAddAWSService<IAmazonS3>();
        serviceCollection.AddTransient<ITo, S3To>();
        return serviceCollection;
    }
}
