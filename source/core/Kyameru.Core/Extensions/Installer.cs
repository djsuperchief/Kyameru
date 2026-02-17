using System;
using Kyameru;
using Kyameru.Core;
using Kyameru.Core.Entities;
using Kyameru.Core.Enums;

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
        
        /// <summary>
        /// Registers a Kyameru dependency. Mostly for internal use by component extensions.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="chainLink">Chain link to add dependency to.</param>
        /// <param name="implementation">Implementation factory.</param>
        /// <typeparam name="TContract">Type of dependency.</typeparam>
        /// <returns>Returns an instance of the <see cref="ChainLinkDependency"/>.</returns>
        /// <remarks>This is a way of registering dependencies for a chain link so that it can be resolved during execution via keyed services.</remarks>
        public static ChainLinkDependency RegisterKyameruDependency<TContract>(this IServiceCollection services, ChainLinkDependencyType chainLink, Func<TContract> implementation)
        {
            var response = new ChainLinkDependency()
            {
                Id = Guid.NewGuid(),
                DependencyType = typeof(TContract),
                ChainLink =  chainLink,
            };

            var boxedFactory = Box<TContract>(implementation, response.Id);
            services.AddKeyedTransient(typeof(TContract), response.Id, boxedFactory);
            return response;
        }
        
        private static Func<IServiceProvider, object, object> Box<T>(Func<T> factory, Guid id) => (provider, o) => factory();
    }
}
