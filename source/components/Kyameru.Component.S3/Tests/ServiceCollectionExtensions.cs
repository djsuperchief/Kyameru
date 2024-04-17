using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.S3.Tests;

public static class ServiceCollectionExtensions
{
    public static bool Contains(this IServiceCollection serviceCollection, Type service, Type implementation)
    {
        return serviceCollection.Any(sd => sd.ServiceType == service && sd.ImplementationType == implementation);
    }
}
