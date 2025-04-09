using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;

namespace Kyameru.Component.File.Tests;

internal class ServiceHelper
{
    public IServiceCollection GetServiceDescriptors()
    {

        IServiceCollection serviceDescriptors = new ServiceCollection();
        serviceDescriptors.AddTransient<ILogger<Kyameru.Route>>(sp =>
        {
            return Substitute.For<ILogger<Kyameru.Route>>();
        });

        Inflator inflator = new Inflator();
        inflator.RegisterFrom(serviceDescriptors);
        inflator.RegisterTo(serviceDescriptors);

        return serviceDescriptors;
    }

    public IServiceProvider GetServiceProvider()
    {
        return this.GetServiceDescriptors().BuildServiceProvider();
    }
}
