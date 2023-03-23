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
            Assert.NotNull(this.AddComponent("CanSetupFullFact"));
        }

        [Fact]
        public async Task CanExecute()
        {
            Component.Test.GlobalCalls.Clear("CanExecute");
            IHostedService service = this.AddComponent("CanExecute");

            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);
            Assert.Equal(7, this.GetCallCount("CanExecute"));
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
            IHostedService service = this.AddComponent("CanExecuteMultipleChains", true);

            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);
            Assert.Equal(20, this.GetCallCount("CanExecuteMultipleChains"));
        }

        [Fact]
        public async Task CanExecuteAtomic()
        {
            IHostedService service = this.GetNoErrorChain("CanExecuteAtomic");
            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);
            Assert.Equal(6, this.GetCallCount("CanExecuteAtomic"));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task AddHeaderErrors(bool secondFunction)
        {
            string testName = $"AddHeaderErrors_{secondFunction.ToString()}";
            Component.Test.GlobalCalls.Clear(testName);
            IHostedService service = this.GetHeaderError(secondFunction, testName);
            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);
            Assert.Equal(1, this.GetCallCount(testName));
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


            this.callPoints.Add("FROM", 1);
            this.callPoints.Add("TO", 2);
            this.callPoints.Add("ATOMIC", 3);
            this.callPoints.Add("COMPONENT", 4);
            this.callPoints.Add("ERROR", 5);
        }

        private int GetCallCount(string test)
        {
            return Component.Test.GlobalCalls.CallDict[test].Sum(x => this.callPoints[x]);
        }

        private IHostedService AddComponent(string test, bool multiChain = false)
        {
            IServiceCollection serviceCollection = this.GetServiceDescriptors();

            this.processComponent.Setup(x => x.Process(It.IsAny<Routable>())).Callback(() =>
            {
                Kyameru.Component.Test.GlobalCalls.AddCall(test, "COMPONENT");
            });
            this.errorComponent.Setup(x => x.Process(It.IsAny<Routable>())).Callback(() =>
            {
                Kyameru.Component.Test.GlobalCalls.AddCall(test, "ERROR");
            });

            if (multiChain)
            {
                Kyameru.Route.From($"Test://hello?TestName={test}")
                    .Process(this.processComponent.Object)
                    .Process(this.processComponent.Object)
                    .To("Test://world")
                    .To("Test://kyameru")
                    .Atomic("Test://plop")
                    .Error(this.errorComponent.Object)
                    .Id("WillNotExecute")
                    .Build(serviceCollection);
            }
            else
            {
                Kyameru.Route.From($"Test://hello?TestName={test}")
                    .Process(this.processComponent.Object)
                    .To("Test://world")
                    .Build(serviceCollection);
            }
            IServiceProvider provider = serviceCollection.BuildServiceProvider();
            return provider.GetService<IHostedService>();
        }

        private IEnumerable<IHostedService> AddTwoRoutes()
        {
            IServiceCollection serviceCollection = this.GetServiceDescriptors();
            Kyameru.Route.From("Test://first?TestName=TwoRoutes")
                    .Process(this.processComponent.Object)
                    .To("Test://world")
                    .Build(serviceCollection);

            Kyameru.Route.From("Test://second?TestName=TwoRoutes")
                    .Process(this.processComponent.Object)
                    .To("Test://world")
                    .Build(serviceCollection);
            IServiceProvider provider = serviceCollection.BuildServiceProvider();
            return provider.GetServices<IHostedService>();
        }

        private IHostedService SetupDIComponent()
        {
            IServiceCollection serviceCollection = this.GetServiceDescriptors();
            Kyameru.Route.From("Test://hello?TestName=DITest")
                .Process<Tests.Mocks.IMyComponent>()
                .Process(this.diProcessor.Object)
                .To("Test://world")
                .Build(serviceCollection);

            IServiceProvider provider = serviceCollection.BuildServiceProvider();
            return provider.GetService<IHostedService>();
        }

        private IHostedService GetNoErrorChain(string test)
        {
            IServiceCollection serviceCollection = this.GetServiceDescriptors();
            Kyameru.Route.From($"Test://hello?TestName={test}")
                .To("Test://world")
                .Atomic("Test://boom")
                .Build(serviceCollection);
            IServiceProvider provider = serviceCollection.BuildServiceProvider();
            return provider.GetService<IHostedService>();
        }

        private IHostedService GetHeaderError(bool dual, string test)
        {
            IServiceCollection serviceCollection = this.GetServiceDescriptors();
            if (!dual)
            {
                Route.From($"Test://hello?TestName={test}")
                    .AddHeader("One", () =>
                    {
                        throw new NotImplementedException("whoops");
                    })
                    .To("Test://world")
                    .Error(this.errorComponent.Object)
                    .Build(serviceCollection);
            }
            else
            {
                Route.From($"Test://hello?TestName={test}")
                    .AddHeader("One", (x) =>
                    {
                        throw new NotImplementedException("whoops");
                    })
                    .To("Test://world")
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