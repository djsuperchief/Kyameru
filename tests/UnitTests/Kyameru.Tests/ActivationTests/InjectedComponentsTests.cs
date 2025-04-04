using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Entities;
using Kyameru.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Kyameru.Tests.ActivationTests
{
    public class InjectedComponentsTests
    {
        private readonly Mock<ILogger<Route>> logger = new Mock<ILogger<Route>>();
        private readonly Mock<IProcessComponent> processComponent = new Mock<IProcessComponent>();

        [Fact]
        public async Task CanActivateAndRun()
        {
            IServiceCollection serviceCollection = this.GetServiceDescriptors();
            Routable routable = null;
            this.processComponent.Reset();
            this.processComponent.Setup(x => x.ProcessAsync(It.IsAny<Routable>(), It.IsAny<CancellationToken>())).Callback((Routable x, CancellationToken c) =>
            {
                routable = x;
            });

            Kyameru.Route.From("injectiontest:///mememe")
                .Process(this.processComponent.Object)
                .To("injectiontest:///somewhere")
                .Build(serviceCollection);


            IServiceProvider provider = serviceCollection.BuildServiceProvider();
            IHostedService service = provider.GetService<IHostedService>();
            var thread = TestThread.CreateNew(service.StartAsync, 2);
            thread.Start();
            thread.WaitForExecution();
            await thread.CancelAsync();

            Assert.Equal("Async Injected Test Complete", routable?.Body);
        }

        [Theory]
        [InlineData("from", "Error activating from component.")]
        [InlineData("to", "Error activating to component.")]
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
                var thread = TestThread.CreateNew(service.StartAsync, 2);
                thread.Start();
                thread.WaitForExecution();
                await thread.CancelAsync();
            }
            catch (Exception ex)
            {
                actual = ex.Message;
            }

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CanActivateProcessingByDomain()
        {
            IServiceCollection serviceCollection = this.GetServiceDescriptors();
            Routable routable = null;
            this.processComponent.Reset();
            this.processComponent.Setup(x => x.ProcessAsync(It.IsAny<Routable>(), It.IsAny<CancellationToken>())).Callback((Routable x, CancellationToken c) =>
            {
                routable = x;
            });

            Kyameru.Route.From("injectiontest:///mememe")
                .Process("Mocks.MyComponent")
                .Process(processComponent.Object)
                .To("injectiontest:///somewhere")
                .Build(serviceCollection);
            IServiceProvider provider = serviceCollection.BuildServiceProvider();
            IHostedService service = provider.GetService<IHostedService>();
            var thread = TestThread.CreateNew(service.StartAsync, 2);
            thread.Start();
            thread.WaitForExecution();
            await thread.CancelAsync();

            Assert.Equal("Yes", routable.Headers["ComponentRan"]);
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
