using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Kyameru.Facts.ActivationFacts
{
    public class IoCFacts
    {
        private readonly Mock<ILogger<Route>> logger = new Mock<ILogger<Route>>();
        private readonly Mock<IProcessComponent> processComponent = new Mock<IProcessComponent>();
        private readonly Mock<IProcessComponent> diProcessor = new Mock<IProcessComponent>();
        private readonly Mock<IErrorComponent> errorComponent = new Mock<IErrorComponent>();
        private readonly Dictionary<string, int> callPoints = new Dictionary<string, int>();

        public IoCFacts()
        {
            Init();
        }

        [Fact]
        public void CanSetupFullFact()
        {
            Assert.NotNull(this.AddComponent());
        }

        [Fact]
        public async Task CanExecute()
        {
            Component.Test.GlobalCalls.Calls.Clear();
            IHostedService service = this.AddComponent();

            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);
            Assert.Equal(7, this.GetCallCount());
        }

        [Fact]
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
            Assert.Equal("Yes", routable.Headers["ComponentRan"]);
        }

        [Fact]
        public async Task CanExecuteMultipleChains()
        {
            Component.Test.GlobalCalls.Calls.Clear();
            IHostedService service = this.AddComponent(true);

            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);
            Assert.Equal(20, this.GetCallCount());
        }

        [Fact]
        public async Task CanExecuteAtomic()
        {
            Component.Test.GlobalCalls.Calls.Clear();
            IHostedService service = this.GetNoErrorChain();
            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);
            Assert.Equal(6, this.GetCallCount());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task AddHeaderErrors(bool secondFunction)
        {
            Component.Test.GlobalCalls.Calls.Clear();
            IHostedService service = this.GetHeaderError(secondFunction);
            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);
            Assert.Equal(6, this.GetCallCount());
        }

        [Fact]
        public async Task MultipleRoutesWork()
        {
            int calls = 0;
            this.processComponent.Reset();
            this.processComponent.Setup(x => x.Process(It.IsAny<Routable>())).Callback((Routable x) =>
            {
                calls++;
            });

            IEnumerable<IHostedService> services = this.AddTwoRoutes();
            Assert.Equal(2, services.Count());
            for (int i = 0; i < services.Count(); i++)
            {
                await services.ElementAt(i).StartAsync(CancellationToken.None);
                await services.ElementAt(i).StopAsync(CancellationToken.None);
            }

            Assert.Equal(2, calls);
        }

        #region Setup

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
                Kyameru.Route.From("Fact://hello")
                    .Process(this.processComponent.Object)
                    .Process(this.processComponent.Object)
                    .To("Fact://world")
                    .To("Fact://kyameru")
                    .Atomic("Fact://plop")
                    .Error(this.errorComponent.Object)
                    .Id("WillNotExecute")
                    .Build(serviceCollection);
            }
            else
            {
                Kyameru.Route.From("Fact://hello")
                    .Process(this.processComponent.Object)
                    .To("Fact://world")
                    .Build(serviceCollection);
            }
            IServiceProvider provider = serviceCollection.BuildServiceProvider();
            return provider.GetService<IHostedService>();
        }

        private IEnumerable<IHostedService> AddTwoRoutes()
        {
            IServiceCollection serviceCollection = this.GetServiceDescriptors();
            Kyameru.Route.From("Fact://first")
                    .Process(this.processComponent.Object)
                    .To("Fact://world")
                    .Build(serviceCollection);

            Kyameru.Route.From("Fact://second")
                    .Process(this.processComponent.Object)
                    .To("Fact://world")
                    .Build(serviceCollection);
            IServiceProvider provider = serviceCollection.BuildServiceProvider();
            return provider.GetServices<IHostedService>();
        }

        private IHostedService SetupDIComponent()
        {
            IServiceCollection serviceCollection = this.GetServiceDescriptors();
            Kyameru.Route.From("Fact://hello")
                .Process<Tests.Mocks.IMyComponent>()
                .Process(this.diProcessor.Object)
                .To("Fact://world")
                .Build(serviceCollection);

            IServiceProvider provider = serviceCollection.BuildServiceProvider();
            return provider.GetService<IHostedService>();
        }

        private IHostedService GetNoErrorChain()
        {
            IServiceCollection serviceCollection = this.GetServiceDescriptors();
            Kyameru.Route.From("Fact://hello")
                .To("Fact://world")
                .Atomic("Fact://boom")
                .Build(serviceCollection);
            IServiceProvider provider = serviceCollection.BuildServiceProvider();
            return provider.GetService<IHostedService>();
        }

        private IHostedService GetHeaderError(bool dual)
        {
            IServiceCollection serviceCollection = this.GetServiceDescriptors();
            if (!dual)
            {
                Route.From("Fact://hello")
                    .AddHeader("One", () =>
                    {
                        throw new NotImplementedException("whoops");
                    })
                    .To("Fact://world")
                    .Error(this.errorComponent.Object)
                    .Build(serviceCollection);
            }
            else
            {
                Route.From("Fact://hello")
                    .AddHeader("One", (x) =>
                    {
                        throw new NotImplementedException("whoops");
                    })
                    .To("Fact://world")
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
            serviceCollection.AddTransient<Tests.Mocks.IMyComponent, Tests.Mocks.MyComponent>();

            return serviceCollection;
        }

        #endregion Setup
    }
}