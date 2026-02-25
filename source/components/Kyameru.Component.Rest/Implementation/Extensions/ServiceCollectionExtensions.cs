using System;
using Kyameru.Component.Rest.Contracts;
using Kyameru.Component.Rest.Implementation;
using Kyameru.Core.Entities;
using Kyameru.Core.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static ChainLinkDependency RegisterKyameruRestAuthApi(this IServiceCollection services, string apiKey, ChainLinkDependencyType chainLink = ChainLinkDependencyType.Unset, string header = "X-API-Key")
        {
            var apiToken = new ApiAuthToken(apiKey, header);
            return services.RegisterKyameruDependency<IAuthStrategy>(chainLink,() => apiToken);
        }

        public static ChainLinkDependency RegisterKyameruRestAuthBearer(this IServiceCollection services, string token,
            ChainLinkDependencyType chainLink = ChainLinkDependencyType.Unset)
        {
            var bearerToken = new BearerAuthToken(token);
            return services.RegisterKyameruDependency<IAuthStrategy>(chainLink,() => bearerToken);
        }

        public static ChainLinkDependency RegisterKyameruRestAuthBasic(this IServiceCollection services, string username,
            string password, ChainLinkDependencyType chainLink = ChainLinkDependencyType.Unset)
        {
            var basicAuth = new BasicAuthToken(username, password);
            return services.RegisterKyameruDependency<IAuthStrategy>(chainLink,() => basicAuth);
        }
    }
}