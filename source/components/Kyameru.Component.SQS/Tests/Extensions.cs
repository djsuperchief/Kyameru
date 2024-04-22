using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.Sqs.Tests;

public static class Extensions
{
    public static bool Contains(this IServiceCollection serviceCollection, Type service, Type implementation)
    {
        return serviceCollection.Any(sd => sd.ServiceType == service && sd.ImplementationType == implementation);
    }
}