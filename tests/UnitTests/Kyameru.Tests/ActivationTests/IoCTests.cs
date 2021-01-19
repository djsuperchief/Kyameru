using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kyameru.Tests.ActivationTests
{
    [TestFixture(Category = "IoC")]
    public class IoCTests
    {
        private readonly Mock<ILogger<Route>> logger = new Mock<ILogger<Route>>();
        private readonly Mock<IProcessComponent> processComponent = new Mock<IProcessComponent>();
        private readonly Mock<IProcessComponent> diProcessor = new Mock<IProcessComponent>();
        private readonly Mock<IErrorComponent> errorComponent = new Mock<IErrorComponent>();
        private Dictionary<string, int> callPoints = new Dictionary<string, int>();

        [Test]
        public void CanSetupFullTest()
        {
            Assert.NotNull(this.AddComponent());
        }

        [Test]
        public async Task CanExecute()
        {
            Component.Test.GlobalCalls.Calls.Clear();
            IHostedService service = this.AddComponent();

            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);
            Assert.AreEqual(7, this.GetCallCount());
        }

        [Test]
        public async Task CanRunDIComponent()
        {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            Routable routable = null;
            this.diProcessor.Reset();
            this.diProcessor.Setup(x => x.Process(It.IsAny<Routable>())).Callback((Routable x) =>
            {
                routable = x;
            });
            IHostedService service = this.SetupDIComponent();

            await service.StartAsync(CancellationToken.None);
            autoResetEvent.WaitOne(TimeSpan.FromSeconds(5));
            await service.StopAsync(CancellationToken.None);
            Assert.AreEqual("Yes", routable.Headers["ComponentRan"]);
        }

        [Test]
        public async Task CanExecuteMultipleChains()
        {
            Component.Test.GlobalCalls.Calls.Clear();
            IHostedService service = this.AddComponent(true);

            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);
            Assert.AreEqual(20, this.GetCallCount());
        }

        [Test]
        public async Task CanExecuteAtomic()
        {
            Component.Test.GlobalCalls.Calls.Clear();
            IHostedService service = this.GetNoErrorChain();
            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);
            Assert.AreEqual(6, this.GetCallCount());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task AddHeaderErrors(bool secondFunction)
        {
            Component.Test.GlobalCalls.Calls.Clear();
            IHostedService service = this.GetHeaderError(secondFunction);
            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);
            Assert.AreEqual(6, this.GetCallCount());
        }

        #region Setup

        [OneTimeSetUp]
        public void Init()
        {
            this.processComponent.Setup(x => x.Process(It.IsAny<Routable>())).Callback(() =>
            {
                Kyameru.Component.Test.GlobalCalls.Calls.Add("COMPONENT");
            });
            this.errorComponent.Setup(x => x.Process(It.IsAny<Routable>())).Callback(() =>
            {
                Kyameru.Component.Test.GlobalCalls.Calls.Add("ERROR");
            });

            this.callPoints.Add("FROM", 1);
            this.callPoints.Add("TO", 2);
            this.callPoints.Add("ATOMIC", 3);
            this.callPoints.Add("COMPONENT", 4);
            this.callPoints.Add("ERROR", 5);
        }

        private int GetCallCount()
        {
            return Component.Test.GlobalCalls.Calls.Where(this.callPoints.ContainsKey).Sum(x => this.callPoints[x]);
        }

        private IHostedService AddComponent(bool multiChain = false)
        {
            IServiceCollection serviceCollection = this.GetServiceDescriptors();
            if (multiChain)
            {
                Kyameru.Route.From("test://hello")
                    .Process(this.processComponent.Object)
                    .Process(this.processComponent.Object)
                    .To("test://world")
                    .To("test://kyameru")
                    .Atomic("test://plop")
                    .Error(this.errorComponent.Object)
                    .Id("WillNotExecute")
                    .Build(serviceCollection);
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

        private IHostedService SetupDIComponent()
        {
            IServiceCollection serviceCollection = this.GetServiceDescriptors();
            Kyameru.Route.From("test://hello")
                .Process<Mocks.IMyComponent>()
                .Process(this.diProcessor.Object)
                .To("test://world")
                .Build(serviceCollection);

            IServiceProvider provider = serviceCollection.BuildServiceProvider();
            return provider.GetService<IHostedService>();
        }

        private IHostedService GetNoErrorChain()
        {
            IServiceCollection serviceCollection = this.GetServiceDescriptors();
            Kyameru.Route.From("test://hello")
                .To("test://world")
                .Atomic("test://boom")
                .Build(serviceCollection);
            IServiceProvider provider = serviceCollection.BuildServiceProvider();
            return provider.GetService<IHostedService>();
        }

        private IHostedService GetHeaderError(bool dual)
        {
            IServiceCollection serviceCollection = this.GetServiceDescriptors();
            if (!dual)
            {
                Route.From("test://hello")
                    .AddHeader("One", () =>
                    {
                        throw new NotImplementedException("whoops");
                    })
                    .To("test://world")
                    .Error(this.errorComponent.Object)
                    .Build(serviceCollection);
            }
            else
            {
                Route.From("test://hello")
                    .AddHeader("One", (x) =>
                    {
                        throw new NotImplementedException("whoops");
                    })
                    .To("test://world")
                    .Error(this.errorComponent.Object)
                    .Build(serviceCollection);
            }
            IServiceProvider provider = serviceCollection.BuildServiceProvider();
            return provider.GetService<IHostedService>();
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

        #endregion Setup
    }
}