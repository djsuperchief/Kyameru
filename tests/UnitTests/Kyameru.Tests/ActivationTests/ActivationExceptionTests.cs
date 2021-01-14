using Kyameru.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kyameru.Tests.ActivationTests
{
    [TestFixture]
    public class ActivationExceptionTests
    {
        private readonly Mock<ILogger<Route>> logger = new Mock<ILogger<Route>>();

        [Test]
        [TestCase("From", "invalid", "test", "test")]
        [TestCase("Atomic", "test", "invalid", "test")]
        [TestCase("To", "test", "test", "invalid")]
        public void ComponentInvalid(string expected, string from, string atomic, string to)
        {
            string errorComponent = string.Empty;
            try
            {
                _ = this.GetHostedService(from, atomic, to);
            }
            catch (ActivationException ex)
            {
                errorComponent = ex.Component;
            }
            Assert.AreEqual(expected, errorComponent);
        }

        private IHostedService GetHostedService(
                string fromHost,
                string atomicHost,
                string toHost)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<ILogger<Kyameru.Route>>(sp =>
            {
                return this.logger.Object;
            });
            string from = $"{fromHost}://hello";
            string to = $"{toHost}://hello";
            string atomic = $"{atomicHost}://hello";

            Kyameru.Route.From(from)
                .To(to)
                .Atomic(atomic)
                .Build(serviceCollection);

            IServiceProvider provider = serviceCollection.BuildServiceProvider();
            return provider.GetService<IHostedService>();
        }
    }
}