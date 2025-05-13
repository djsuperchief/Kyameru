using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Kyameru.Tests;

public abstract class BaseTests
{
    protected IHostedService BuildAndGetServices(Core.Builder kyameruBuilder, Component.Generic.Builder componentBuilder)
    {
        var services = GetServiceDescriptors();
        componentBuilder.Build(services);
        kyameruBuilder.Build(services);
        return services.BuildServiceProvider().GetRequiredService<IHostedService>();
    }

    protected IServiceCollection GetServiceDescriptors()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ILogger<Kyameru.Route>>(sp =>
        {
            return Substitute.For<ILogger<Kyameru.Route>>();
        });
        serviceCollection.AddTransient<Tests.Mocks.IMyComponent, Tests.Mocks.MyComponent>();

        return serviceCollection;
    }
}
