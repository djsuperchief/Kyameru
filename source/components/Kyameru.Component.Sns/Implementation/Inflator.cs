﻿using Amazon.SimpleNotificationService;
using Kyameru.Core.Exceptions;
using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System;

namespace Kyameru.Component.Sns
{
    public class Inflator : IOasis
    {
        public IFromChainLink CreateFromComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            throw new RouteNotAvailableException(string.Format(Core.Resources.ERROR_ROUTE_UNAVAILABLE, "FROM", "SNS"));
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
            throw new RouteNotAvailableException(string.Format(Core.Resources.ERROR_ROUTE_UNAVAILABLE, "FROM", "SNS"));
        }

        public IServiceCollection RegisterScheduled(IServiceCollection serviceCollection)
        {
            throw new NotImplementedException();
        }

        public IServiceCollection RegisterTo(IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddAWSService<IAmazonSimpleNotificationService>();
            serviceCollection.AddTransient<ITo, SnsTo>();

            return serviceCollection;
        }
    }
}
