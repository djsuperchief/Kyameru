﻿using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Kyameru.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Kyameru.Tests.ActivationTests
{
    public class ExceptionTests
    {
        private readonly Mock<ILogger<Route>> logger = new Mock<ILogger<Route>>();
        private readonly Mock<IErrorComponent> errorComponent = new Mock<IErrorComponent>();
        private readonly Mock<IProcessComponent> processComponent = new Mock<IProcessComponent>();

        [Fact]
        public async Task FromException()
        {
            Routable routable = null;

            this.errorComponent.Reset();
            this.errorComponent.Setup(x => x.ProcessAsync(It.IsAny<Routable>(), It.IsAny<CancellationToken>())).Callback((Routable x, CancellationToken c) =>
            {
                routable = x;
            });
            this.processComponent.Reset();

            IHostedService service = this.GetHostedService(SetupChain, true);
            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);

            Assert.Null(routable);
        }

        [Fact]
        public async Task FromRaiseException()
        {
            await Assert.ThrowsAsync<NotImplementedException>(async () =>
            {
                IHostedService service = this.GetHostedService(SetupBubbleChain, true);
                await service.StartAsync(CancellationToken.None);
                await service.StopAsync(CancellationToken.None);
            });
        }

        [Fact]
        public async Task ComponentError()
        {
            Routable routable = null;

            this.errorComponent.Reset();
            this.errorComponent.Setup(x => x.ProcessAsync(It.IsAny<Routable>(), It.IsAny<CancellationToken>())).Callback((Routable x, CancellationToken c) =>
            {
                routable = x;
            });
            this.processComponent.Reset();
            this.processComponent.Setup(x => x.ProcessAsync(It.IsAny<Routable>(), It.IsAny<CancellationToken>())).Callback((Routable x, CancellationToken c) =>
            {
                throw new Kyameru.Core.Exceptions.ProcessException("Manual Error");
            });

            IHostedService service = this.GetHostedService(SetupChain);
            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);

            Assert.True(this.IsInError(routable, "Processing component"));
        }

        [Fact]
        public async Task ToError()
        {
            Routable routable = null;

            this.errorComponent.Reset();
            this.errorComponent.Setup(x => x.ProcessAsync(It.IsAny<Routable>(), It.IsAny<CancellationToken>())).Callback((Routable x, CancellationToken c) =>
            {
                routable = x;
            });
            this.processComponent.Reset();

            IHostedService service = this.GetHostedService(SetupChain, false, true);
            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);

            Assert.True(this.IsInError(routable, "To Component"));
        }

        [Fact]
        public async Task AtomicError()
        {
            Routable routable = null;

            this.errorComponent.Reset();
            this.errorComponent.Setup(x => x.ProcessAsync(It.IsAny<Routable>(), It.IsAny<CancellationToken>())).Callback((Routable x, CancellationToken c) =>
            {
                routable = x;
            });
            this.processComponent.Reset();

            IHostedService service = this.GetHostedService(SetupChain, false, false, true);
            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);

            Assert.True(this.IsInError(routable, "Atomic Component"));
        }

        [Fact]
        public async Task ErrorComponentErrors()
        {
            Routable routable = null;
            this.errorComponent.Reset();
            this.errorComponent.Setup(x => x.ProcessAsync(It.IsAny<Routable>(), It.IsAny<CancellationToken>())).Callback((Routable x, CancellationToken c) =>
            {
                routable = x;
                throw new ProcessException("Manual Error", new IndexOutOfRangeException("Random index"));
            });

            IHostedService service = this.GetHostedService(SetupChain, false, false, true);
            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);

            Assert.True(this.IsInError(routable, "Error Component"));
        }

        private bool IsInError(Routable routable, string component)
        {
            return routable != null
                && routable.Error.Component == component
                && routable.Error.CurrentAction == "Handle"
                && routable.Error.Message == "Manual Error";
        }

        private IHostedService GetHostedService(
            Action<string, string, string, IServiceCollection> setup,
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
            string to = $"error://path:test@test.com?Error={toError}";
            string atomic = $"error://path?Error={atomicError}";

            setup.Invoke(from, to, atomic, serviceCollection);

            IServiceProvider provider = serviceCollection.BuildServiceProvider();
            return provider.GetService<IHostedService>();
        }

        private void SetupChain(string from, string to, string atomic, IServiceCollection serviceDescriptors)
        {
            Kyameru.Route.From(from)
                .Process(this.processComponent.Object)
                .To(to)
                .Atomic(atomic)
                .Error(this.errorComponent.Object)
                .Build(serviceDescriptors);
        }

        private void SetupBubbleChain(string from, string to, string atomic, IServiceCollection serviceDescriptors)
        {
            Kyameru.Route.From(from)
                .Process(this.processComponent.Object)
                .To(to)
                .Atomic(atomic)
                .Error(this.errorComponent.Object)
                .RaiseExceptions()
                .Build(serviceDescriptors);
        }
    }
}