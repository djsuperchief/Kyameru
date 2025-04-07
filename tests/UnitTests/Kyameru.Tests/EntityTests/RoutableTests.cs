using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Kyameru.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Kyameru.Tests.EntityTests
{
    public class RoutableTests
    {
        private readonly Mock<ILogger<Route>> logger = new Mock<ILogger<Route>>();
        private readonly Mock<IProcessComponent> component = new Mock<IProcessComponent>();

        [Fact]
        public void CreatedHeaderError()
        {
            Routable routable = this.CreateMessage();
            Assert.Throws<Kyameru.Core.Exceptions.CoreException>(() => routable.SetHeader("Test", "changed"));
        }

        [Fact]
        public void UserImmutableThrowsHeader()
        {
            Routable routable = this.CreateMessage();
            routable.SetHeader("&Nope", "Nope");
            Assert.Throws<Kyameru.Core.Exceptions.CoreException>(() => routable.SetHeader("Nope", "yep"));
        }

        [Fact]
        public void UserMutableWorks()
        {
            Routable routable = this.CreateMessage();
            routable.SetHeader("FileType", "txt");
            routable.SetHeader("FileType", "jpg");
            Assert.Equal("jpg", routable.Headers["FileType"]);
        }

        [Fact]
        public void SetBodyWorks()
        {
            string body = "body text";
            Routable routable = this.CreateMessage();
            routable.SetBody<string>(body);
            Assert.Equal(body, routable.Body);
        }

        [Theory]
        [MemberData(nameof(BodyTestCases))]
        public void SetBodyWorksWithHeader(IBodyTests bodyTest)
        {
            Assert.True(bodyTest.IsEqual(this.CreateMessage()));
        }

        [Theory]
        [InlineData("TO", true)]
        [InlineData("ATOMIC", false)]
        public async Task ProcessExitWorks(string call, bool setupComponent)
        {
            string testName = $"ProcessExitWorks_{call}";
            this.component.Reset();
            this.component.Setup(x => x.ProcessAsync(It.IsAny<Routable>(), It.IsAny<CancellationToken>())).Callback((Routable x, CancellationToken c) =>
            {
                x.SetHeader("SetExit", "true");
                if (setupComponent)
                {
                    x.SetExitRoute("Manually triggered exit");
                }
            });

            Assert.False(await this.RunProcess(call, testName));
        }

        public static IEnumerable<object[]> BodyTestCases()
        {
            yield return new object[] { new BodyTests<string>("test", "String") };
            yield return new object[] { new BodyTests<int>(1, "Int32") };
        }

        private async Task<bool> RunProcess(string callsContain, string test)
        {
            IHostedService service = this.GetRoute(test);
            var thread = TestThread.CreateNew(service.StartAsync, 3);
            thread.StartAndWait();
            await thread.CancelAsync();

            return Kyameru.Component.Test.GlobalCalls.CallDict[test].Contains(callsContain);
        }

        private Routable CreateMessage()
        {
            return new Routable(new System.Collections.Generic.Dictionary<string, string>()
            {
                {"&Test", "value" }
            }, "test");
        }

        private IHostedService GetRoute(string test)
        {
            IServiceCollection serviceCollection = this.GetServiceDescriptors();
            Kyameru.Route.From($"test://hello?TestName={test}")
                .Process(this.component.Object)
                .To("test://world")
                .Atomic("test://boom")
                .Build(serviceCollection);
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

            return serviceCollection;
        }
    }
}