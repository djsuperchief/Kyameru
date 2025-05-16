using Kyameru.Core.Exceptions;
using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Kyameru.Component.Slack
{
    /// <summary>
    /// Implementation of inflator.
    /// </summary>
    public class Inflator : IOasis
    {
        /// <summary>
        /// Creates a from component.
        /// </summary>
        /// <param name="headers">Incoming headers.</param>]
        /// <returns>Returns a new instance of a <see cref="IFromChainLink"/> class.</returns>
        public IFromChainLink CreateFromComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            throw new RouteNotAvailableException(string.Format(Core.Resources.ERROR_ROUTE_UNAVAILABLE, "FROM", "Slack"));
        }

        public IScheduleChainLink CreateScheduleComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a to component.
        /// </summary>
        /// <param name="headers">Incoming headers.</param>
        /// <returns>Returns a new instance of a <see cref="IToChainLink"/> class.</returns>
        public IToChainLink CreateToComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            return new SlackTo(headers);
        }

        public IServiceCollection RegisterFrom(IServiceCollection serviceCollection)
        {
            throw new RouteNotAvailableException(string.Format(Core.Resources.ERROR_ROUTE_UNAVAILABLE, "FROM", "Slack"));
        }

        public IServiceCollection RegisterScheduled(IServiceCollection serviceCollection)
        {
            throw new NotImplementedException();
        }

        public IServiceCollection RegisterTo(IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }
    }
}