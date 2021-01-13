using Kyameru.Core.Contracts;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Kyameru.Tests.ActivationTests
{
    [TestFixture(Category = "ActivationTests")]
    public class RouteTests
    {
        private Mock<IErrorComponent> errorComponent = new Mock<IErrorComponent>();
        private Mock<IProcessComponent> processingComponent = new Mock<IProcessComponent>();

        [Test]
        public void CanAddHeader()
        {
            Core.RouteBuilder route = this.CreateRoute();
            route.AddHeader("Test", "Test");
            Assert.IsTrue(route.ComponentCount == 1);
        }

        [Test]
        public void CanAddHeaderAction()
        {
            Core.RouteBuilder route = this.CreateRoute();
            route.AddHeader("Test", (x) =>
            {
                return x.Headers["Target"];
            });
            Assert.IsTrue(route.ComponentCount == 1);
        }

        [Test]
        public void CanAddHeaderActionTwo()
        {
            Core.RouteBuilder route = this.CreateRoute();
            route.AddHeader("Test", () =>
            {
                return "World";
            });
            Assert.IsTrue(route.ComponentCount == 1);
        }

        [Test]
        public void CanAddProcessingComponent()
        {
            Core.RouteBuilder route = this.CreateRoute();
            route.Process(this.processingComponent.Object);
            Assert.IsTrue(route.ComponentCount == 1);
        }

        [Test]
        public void CanCreateTo()
        {
            Core.Builder builder = this.CreateTo(this.CreateRoute());
            Assert.IsTrue(builder.ToComponentCount == 1);
        }

        [Test]
        public void CanChainTwoTo()
        {
            Core.Builder builder = this.CreateTo(this.CreateRoute());
            builder.To("test://world?myHeader=Test");
            Assert.IsTrue(builder.ToComponentCount == 2);
        }

        [Test]
        public void CanSetupError()
        {
            Core.Builder builder = this.CreateTo(this.CreateRoute());
            builder.Error(this.errorComponent.Object);
            Assert.IsTrue(builder.WillProcessError);
        }

        [Test]
        public void CanSetupAtomic()
        {
            Core.Builder builder = this.CreateTo(this.CreateRoute());
            builder = this.CreateAtomic(builder);
            Assert.IsTrue(builder.IsAtomic);
        }

        [Test]
        public void ToThrowsException()
        {
            Assert.Throws<Core.Exceptions.ActivationException>(() =>
           {
               this.CreateTo(this.CreateRoute(), "invalid://nope");
           });
        }

        [Test]
        public void RouteBuilderThrowsException()
        {
            Assert.Throws<Core.Exceptions.RouteUriException>(() => { this.CreateRoute(string.Empty); });
        }

        private Core.RouteBuilder CreateRoute(string route = "test://hello")
        {
            return Route.From(route);
        }

        private Core.Builder CreateTo(Core.RouteBuilder builder, string route = "test://world")
        {
            return builder.To(route);
        }

        private Core.Builder CreateAtomic(Core.Builder builder, string route = "test://hello")
        {
            return builder.Atomic(route);
        }
    }
}