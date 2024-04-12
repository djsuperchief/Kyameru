using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kyameru.Tests.ActivationTests
{
    public class RouteTests
    {
        private readonly Mock<IErrorComponent> errorComponent = new Mock<IErrorComponent>();
        private readonly Mock<IProcessComponent> processingComponent = new Mock<IProcessComponent>();

        [Fact]
        public void CanAddHeader()
        {
            Core.RouteBuilder route = this.CreateRoute();
            route.AddHeader("Test", "Test");
            Assert.True(route.ComponentCount == 1);
        }

        [Fact]
        public void CanAddHeaderAction()
        {
            Core.RouteBuilder route = this.CreateRoute();
            route.AddHeader("Test", (x) =>
            {
                return x.Headers["Target"];
            });
            Assert.True(route.ComponentCount == 1);
        }

        [Fact]
        public void CanAddHeaderActionTwo()
        {
            Core.RouteBuilder route = this.CreateRoute();
            route.AddHeader("Test", () =>
            {
                return "World";
            });
            Assert.True(route.ComponentCount == 1);
        }

        [Fact]
        public void CanAddProcessingComponent()
        {
            Core.RouteBuilder route = this.CreateRoute();
            route.Process(this.processingComponent.Object);
            Assert.True(route.ComponentCount == 1);
        }

        [Fact]
        public void CanAddProcessComponentByDelegate()
        {
            var route = this.CreateRoute();
            route.Process((Routable item) => {
                item.SetHeader("Test", "Test");
            });
            Assert.True(route.ComponentCount == 1);
        }

        [Fact]
        public void CanCreateTo()
        {
            Core.Builder builder = this.CreateTo(this.CreateRoute());
            Assert.True(builder.ToComponentCount == 1);
        }

        [Fact]
        public void CanChainTwoTo()
        {
            Core.Builder builder = this.CreateTo(this.CreateRoute());
            builder.To("test://world?myHeader=Test");
            Assert.True(builder.ToComponentCount == 2);
        }

        [Fact]
        public void CanSetupError()
        {
            Core.Builder builder = this.CreateTo(this.CreateRoute());
            builder.Error(this.errorComponent.Object);
            Assert.True(builder.WillProcessError);
        }

        [Fact]
        public void CanSetupAtomic()
        {
            Core.Builder builder = this.CreateTo(this.CreateRoute());
            builder = this.CreateAtomic(builder);
            Assert.True(builder.IsAtomic);
        }

        [Fact]
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