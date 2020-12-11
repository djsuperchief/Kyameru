using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
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
    [TestFixture(Category = "Exceptions")]
    public class ExceptionTests
    {
        private readonly Mock<ILogger<Route>> logger = new Mock<ILogger<Route>>();
        private readonly Mock<IErrorComponent> errorComponent = new Mock<IErrorComponent>();
        private readonly Mock<IProcessComponent> processComponent = new Mock<IProcessComponent>();

        [Test]
        public async Task FromException()
        {
            Routable routable = null;

            this.errorComponent.Reset();
            this.errorComponent.Setup(x => x.Process(It.IsAny<Routable>())).Callback((Routable x) =>
            {
                routable = x;
            });
            this.processComponent.Reset();

            IHostedService service = this.GetHostedService(true);
            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);

            Assert.IsNull(routable);
        }

        [Test]
        public async Task ComponentError()
        {
            Routable routable = null;

            this.errorComponent.Reset();
            this.errorComponent.Setup(x => x.Process(It.IsAny<Routable>())).Callback((Routable x) =>
            {
                routable = x;
            });
            this.processComponent.Reset();
            this.processComponent.Setup(x => x.Process(It.IsAny<Routable>())).Callback((Routable x) =>
            {
                routable = x;
                throw new NotImplementedException();
            });

            IHostedService service = this.GetHostedService();
            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);

            Assert.IsTrue(this.IsInError(routable, "Processing component"));
        }

        [Test]
        public async Task ToError()
        {
            Routable routable = null;

            this.errorComponent.Reset();
            this.errorComponent.Setup(x => x.Process(It.IsAny<Routable>())).Callback((Routable x) =>
            {
                routable = x;
            });
            this.processComponent.Reset();

            IHostedService service = this.GetHostedService(false, true);
            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);

            Assert.IsTrue(this.IsInError(routable, "To Component"));
        }

        [Test]
        public async Task AtomicError()
        {
            Routable routable = null;

            this.errorComponent.Reset();
            this.errorComponent.Setup(x => x.Process(It.IsAny<Routable>())).Callback((Routable x) =>
            {
                routable = x;
            });
            this.processComponent.Reset();

            IHostedService service = this.GetHostedService(false, false, true);
            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);

            Assert.IsTrue(this.IsInError(routable, "Atomic Component"));
        }

        private bool IsInError(Routable routable, string component)
        {
            return routable != null && routable.Error.Component == component;
        }

        private IHostedService GetHostedService(
            bool fromError = false,
            bool toError = false,
            bool atomicError = false)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<ILogger<Kyameru.Route>>(sp =>
            {
                return this.logger.Object;
            });
            string from = $"error://path?Error={fromError}";
            string to = $"error://path?Error={toError}";
            string atomic = $"error://path?Error={atomicError}";

            Kyameru.Route.From(from)
                .Process(this.processComponent.Object)
                .To(to)
                .Atomic(atomic)
                .Error(this.errorComponent.Object)
                .Build(serviceCollection);

            IServiceProvider provider = serviceCollection.BuildServiceProvider();
            return provider.GetService<IHostedService>();
        }
    }
}