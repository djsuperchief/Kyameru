using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Kyameru.Tests.EntityTests
{
    [TestFixture]
    public class RoutableTests
    {
        private readonly Mock<ILogger<Route>> logger = new Mock<ILogger<Route>>();
        private readonly Mock<IProcessComponent> component = new Mock<IProcessComponent>();

        [Test]
        public void CreatedHeaderError()
        {
            Routable routable = this.CreateMessage();
            Assert.Throws<Kyameru.Core.Exceptions.CoreException>(() => routable.SetHeader("Test", "changed"));
        }

        [Test]
        public void UserImmutableThrowsHeader()
        {
            Routable routable = this.CreateMessage();
            routable.SetHeader("&Nope", "Nope");
            Assert.Throws<Kyameru.Core.Exceptions.CoreException>(() => routable.SetHeader("Nope", "yep"));
        }

        [Test]
        public void UserMutableWorks()
        {
            Routable routable = this.CreateMessage();
            routable.SetHeader("FileType", "txt");
            routable.SetHeader("FileType", "jpg");
            Assert.AreEqual("jpg", routable.Headers["FileType"]);
        }

        [Test]
        public void SetBodyWorks()
        {
            string body = "body text";
            Routable routable = this.CreateMessage();
            routable.SetBody<string>(body);
            Assert.AreEqual(body, routable.Body);
        }

        [Test]
        [TestCase("TO", true)]
        [TestCase("ATOMIC", false)]
        public async Task ProcessExitWorks(string call, bool setupComponent)
        {
            this.component.Reset();
                this.component.Setup(x => x.Process(It.IsAny<Routable>())).Callback((Routable x) =>
                {
                    x.SetHeader("SetExit", "true");
                    if (setupComponent)
                    {
                        x.SetExitRoute("Manually triggered exit");
                    }
                });
            
            Assert.IsFalse(await this.RunProcess(call));
        }

        private async Task<bool> RunProcess(string callsContain)
        {
            Component.Test.GlobalCalls.Calls.Clear();
            IHostedService service = this.GetRoute();
            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);

            return Kyameru.Component.Test.GlobalCalls.Calls.Contains(callsContain);
        }

        private Routable CreateMessage()
        {
            return new Routable(new System.Collections.Generic.Dictionary<string, string>()
            {
                {"&Test", "value" }
            }, "test");
        }

        private IHostedService GetRoute()
        {
            IServiceCollection serviceCollection = this.GetServiceDescriptors();
            Kyameru.Route.From("test://hello")
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