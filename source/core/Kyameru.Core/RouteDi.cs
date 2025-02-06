using System;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Core
{
    /// <summary>
    /// Facility to install directly using service provider.
    /// </summary>
    public class RouteDi
    {
        private readonly IServiceCollection _services;

        /// <summary>
        /// Instantiates a new instance of the <see cref="RouteDi"/> class.
        /// </summary>
        /// <param name="services">Service Descriptors</param>
        public RouteDi(IServiceCollection services)
        {
            _services = services;
        }

        /// <summary>
        /// Loads Kyameru routes from config.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        /// <returns>Returns the service collection.</returns>
        public IServiceCollection FromConfiguration(IConfiguration configuration)
        {
            var kyameruConfig = configuration.GetSection("Kyameru").Get<RouteConfig>();
            Route.FromConfig(kyameruConfig, _services);
            return _services;
        }
    }
}
