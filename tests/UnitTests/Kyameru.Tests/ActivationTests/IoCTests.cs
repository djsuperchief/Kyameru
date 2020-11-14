using Kyameru.Core.Contracts;
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
    [TestFixture(Category = "IoC")]
    public class IoCTests
    {
        private readonly IServiceCollection serviceCollection = new ServiceCollection();
        private readonly Mock<ILogger<Route>> logger = new Mock<ILogger<Route>>();
        private readonly Mock<IProcessComponent> processComponent = new Mock<IProcessComponent>();
        private readonly Mock<IErrorComponent> errorComponent = new Mock<IErrorComponent>();

        [Test]
        public void CanSetupFullTest()
        {
            Assert.NotNull(this.AddComponent());
        }

        [Test]
        public async Task CanExecute()
        {
            IHostedService service = this.AddComponent();

            await service.StartAsync(CancellationToken.None);
            this.logger.Verify();
            await service.StopAsync(CancellationToken.None);
            Assert.AreEqual(5, this.logger.Invocations.Count);
        }

        [Test]
        public async Task CanExecuteMultipleChains()
        {
            IHostedService service = this.AddComponent(true);

            await service.StartAsync(CancellationToken.None);
            this.logger.Verify();
            await service.StopAsync(CancellationToken.None);
            Assert.AreEqual(13, this.logger.Invocations.Count);
        }

        [SetUp]
        public void Init()
        {
            this.serviceCollection.AddTransient<ILogger<Kyameru.Route>>(sp =>
            {
                return this.logger.Object;
            });
        }

        private IHostedService AddComponent(bool multiChain = false)
        {
            if (multiChain)
            {
                Kyameru.Route.From("test://hello")
                    .Process(this.processComponent.Object)
                    .Process(this.processComponent.Object)
                    .To("test://world")
                    .To("test://kyameru")
                    .Error(this.errorComponent.Object)
                    .Build(this.serviceCollection);
            }
            else
            {
                Kyameru.Route.From("test://hello")
                    .Process(this.processComponent.Object)
                    .To("test://world")
                    .Build(serviceCollection);
            }
            IServiceProvider provider = serviceCollection.BuildServiceProvider();
            return provider.GetService<IHostedService>();
        }
    }
}