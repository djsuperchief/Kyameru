using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Component.File.Tests
{
    internal class ServiceHelper
    {
        private readonly Mock<ILogger<Kyameru.Route>> logger = new Mock<ILogger<Route>>();
        public IServiceCollection GetServiceDescriptors()
        {

            IServiceCollection serviceDescriptors = new ServiceCollection();
            serviceDescriptors.AddTransient<ILogger<Kyameru.Route>>(sp =>
            {
                return this.logger.Object;
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
}
