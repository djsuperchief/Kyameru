using Kyameru.Component.Ftp.Contracts;
using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Kyameru.Component.Ftp.Tests
{
    public class InflatorTests
    {
        private readonly Mock<ILogger<Kyameru.Route>> logger = new Mock<ILogger<Route>>();


        [Fact]
        public void ActivateFromWorks()
        {
            IServiceProvider serviceProvider = this.GetServiceProvider();
            IFromComponent fromComponent = new Ftp.Inflator().CreateFromComponent(this.GetHeaders(), false, serviceProvider);
            Assert.NotNull(fromComponent);
        }

        [Fact]
        public void AtomicThrows()
        {
            IServiceProvider serviceProvider = this.GetServiceProvider();
            Assert.Throws<NotImplementedException>(() => new Ftp.Inflator().CreateAtomicComponent(this.GetHeaders()));
        }

        [Fact]
        public void ActivateToWorks()
        {
            IServiceProvider serviceProvider = this.GetServiceProvider();
            IToComponent toComponent = new Ftp.Inflator().CreateToComponent(this.GetHeaders(), serviceProvider);
            Assert.NotNull(toComponent);
        }

        private Dictionary<string, string> GetHeaders()
        {
            return new Dictionary<string, string>()
            {
                { "Host", "127.0.0.1" },
                { "Target", "Test" },
                { "Port", "21" },
                { "PollTime", "1" },
                { "Filter", "*" },
                { "UserName", "test" },
                { "Recursive", "true" },
                { "Delete", "false" },
            };
        }


        private IServiceCollection GetServiceDescriptors()
        {
            IServiceCollection serviceDescriptors = new ServiceCollection();
            serviceDescriptors.AddTransient<ILogger<Kyameru.Route>>(sp =>
            {
                return this.logger.Object;
            });

            Inflator inflator = new Inflator();
            inflator.RegisterTo(serviceDescriptors);
            inflator.RegisterFrom(serviceDescriptors);

            return serviceDescriptors;
        }

        private IServiceProvider GetServiceProvider()
        {
            return this.GetServiceDescriptors().BuildServiceProvider();
        }
    }
}