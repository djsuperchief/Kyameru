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
        private readonly Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
        private readonly Mock<ILogger<Route>> logger = new Mock<ILogger<Route>>();
        private readonly Mock<IProcessComponent> processComponent = new Mock<IProcessComponent>();

        [Test]
        public void CanSetupFullTest()
        {
            Assert.NotNull(this.AddComponent());
        }

        [Test]
        public async Task CanExecute()
        {
            IHostedService service = this.AddComponent();

            bool isExecuted = false;
            await service.StartAsync(CancellationToken.None);
            this.logger.Verify();
            await service.StopAsync(CancellationToken.None);
            Assert.AreEqual(4, this.logger.Invocations.Count);
        }

        [SetUp]
        public void Init()
        {
            this.serviceCollection.AddTransient<ILogger<Kyameru.Route>>(sp =>
            {
                return this.logger.Object;
            });
        }

        private IHostedService AddComponent()
        {
            Kyameru.Route.From("test://hello")
                .Process(this.processComponent.Object)
                .To("test://world")
                .Build(serviceCollection);
            IServiceProvider provider = serviceCollection.BuildServiceProvider();
            return provider.GetService<IHostedService>();
        }
    }
}