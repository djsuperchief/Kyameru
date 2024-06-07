using Kyameru.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using Kyameru.Core.Exceptions;
using Xunit;

namespace Kyameru.Component.Slack.Tests
{
    public class InflatorTests
    {
        private readonly Mock<ILogger<Kyameru.Route>> logger = new Mock<ILogger<Route>>();

        [Fact]
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

        [Fact]
        public void ActivateFromThrows()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                { "Target", "test" }
            };
            Slack.Inflator inflator = new Inflator();
            Assert.Throws<RouteNotAvailableException>(() => { inflator.CreateFromComponent(headers, false, this.GetServiceProvider()); });
        }

        [Fact]
        public void ActivateAtomicThrows()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                { "Target", "test" },
                { "MessageSource", "Body" }
            };
            Slack.Inflator inflator = new Inflator();
            Assert.Throws<RouteNotAvailableException>(() => { inflator.CreateAtomicComponent(headers); });
        }

        [Fact]
        public void RegisterFromThrows()
        {
            Assert.Throws<RouteNotAvailableException>(() => this.GetServiceDescriptors(true));
        }


        private IServiceCollection GetServiceDescriptors(bool tryFrom = false)
        {

            IServiceCollection serviceDescriptors = new ServiceCollection();
            serviceDescriptors.AddTransient<ILogger<Kyameru.Route>>(sp =>
            {
                return this.logger.Object;
            });

            Inflator inflator = new Inflator();
            inflator.RegisterTo(serviceDescriptors);
            if (tryFrom)
            {
                inflator.RegisterFrom(serviceDescriptors);
            }

            return serviceDescriptors;

        }

        private IServiceProvider GetServiceProvider()
        {
            return this.GetServiceDescriptors().BuildServiceProvider();
        }



    }
}