﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Kyameru.Tests.ActivationTests
{
    [TestFixture(Category = "IoC")]
    public class InjectedComponentsTests
    {
        private readonly Mock<ILogger<Route>> logger = new Mock<ILogger<Route>>();
        private readonly Mock<IProcessComponent> processComponent = new Mock<IProcessComponent>();

        [Test]
        public async Task CanActivateAndRun()
        {
            IServiceCollection serviceCollection = this.GetServiceDescriptors();
            Routable routable = null;
            this.processComponent.Reset();
            this.processComponent.Setup(x => x.Process(It.IsAny<Routable>())).Callback((Routable x) =>
            {
                routable = x;
            });

            Kyameru.Route.From("injectiontest:///mememe")
                .Process(this.processComponent.Object)
                .To("injectiontest:///somewhere")
                .Build(serviceCollection);


            IServiceProvider provider = serviceCollection.BuildServiceProvider();
            IHostedService service = provider.GetService<IHostedService>();
            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);

            Assert.AreEqual("Injected Test Complete", routable?.Body);
        }

        [Test]
        [TestCase("from", "Error activating from component.")]
        [TestCase("to", "Error activating to component.")]
        public async Task CanComponentsStart(string componentToError, string expected)
        {
            IServiceCollection serviceCollection = this.GetServiceDescriptors();
            string fromHeaders = componentToError == "from" ? "?WillError=true" : string.Empty;
            string toHeaders = componentToError == "to" ? "?WillError=true" : string.Empty;
            string actual = string.Empty;
            try
            {
                Kyameru.Route.From($"injectiontest:///mememe{fromHeaders}")
                    .Process(this.processComponent.Object)
                    .To($"injectiontest:///somewhere{toHeaders}")
                    .Build(serviceCollection);

                IServiceProvider provider = serviceCollection.BuildServiceProvider();
                IHostedService service = provider.GetService<IHostedService>();
                await service.StartAsync(CancellationToken.None);
                await service.StopAsync(CancellationToken.None);
            }
            catch(Exception ex)
            {
                actual = ex.Message;
            }

            Assert.AreEqual(expected, actual);
        }

        private IServiceCollection GetServiceDescriptors()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<ILogger<Kyameru.Route>>(sp =>
            {
                return this.logger.Object;
            });
            serviceCollection.AddTransient<Mocks.IMyComponent, Mocks.MyComponent>();

            return serviceCollection;
        }
    }

    
}
