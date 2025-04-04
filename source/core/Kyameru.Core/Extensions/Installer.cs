using System;
using Kyameru;
using Kyameru.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions for DI.
    /// </summary>
    public static class Installer
    {
        /// <summary>
        /// Entry point for Kyameru
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <returns>Returns a route builder.</returns>
        public static RouteDi Kyameru(this IServiceCollection services)
        {
            return new RouteDi(services);
        }
    }
}
