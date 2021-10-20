using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Kyameru.Component.Slack.Tests
{
    [TestFixture]
    public class InflatorTests
    {
        private readonly Mock<ILogger<Kyameru.Route>> logger = new Mock<ILogger<Route>>();
        
        [Test]
        public void ActivateToWorks()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                { "Target", "test" },
                { "MessageSource", "Body" }
            };
            Slack.Inflator inflator = new Inflator();
            Assert.NotNull(inflator.CreateToComponent(headers, this.GetServiceProvider()));
        }

        [Test]
        public void ActivateFromThrows()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                { "Target", "test" }
            };
            Slack.Inflator inflator = new Inflator();
            Assert.Throws<NotImplementedException>(() => { inflator.CreateFromComponent(headers, false, this.GetServiceProvider()); });
        }

        [Test]
        public void ActivateAtomicThrows()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                { "Target", "test" },
                { "MessageSource", "Body" }
            };
            Slack.Inflator inflator = new Inflator();
            Assert.Throws<NotImplementedException>(() => { inflator.CreateAtomicComponent(headers); });
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

            return serviceDescriptors;
        }

        private IServiceProvider GetServiceProvider()
        {
            return this.GetServiceDescriptors().BuildServiceProvider();
        }



    }
}