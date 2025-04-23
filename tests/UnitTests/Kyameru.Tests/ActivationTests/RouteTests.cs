using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Kyameru.Core.Enums;
using NSubstitute;
using Xunit;

namespace Kyameru.Tests.ActivationTests
{
    public class RouteTests
    {
        [Fact]
        public void CanAddHeader()
        {
            var route = this.CreateRoute();
            route.AddHeader("Test", "Test");
            Assert.True(route.ComponentCount == 1);
        }

        [Fact]
        public void CanAddHeaderAction()
        {
            var route = this.CreateRoute();
            route.AddHeader("Test", (x) =>
            {
                return x.Headers["Target"];
            });
            Assert.True(route.ComponentCount == 1);
        }

        [Fact]
        public void CanAddHeaderActionTwo()
        {
            var route = this.CreateRoute();
            route.AddHeader("Test", () =>
            {
                return "World";
            });
            Assert.True(route.ComponentCount == 1);
        }

        [Fact]
        public void CanAddProcessingComponent()
        {
            var processingComponent = Substitute.For<IProcessor>();
            var route = this.CreateRoute();
            route.Process(processingComponent);
            Assert.True(route.ComponentCount == 1);
        }

        [Fact]
        public void CanAddProcessComponentByDelegate()
        {
            var route = this.CreateRoute();
            route.Process((Routable item) =>
            {
                item.SetHeader("Test", "Test");
            });
            Assert.True(route.ComponentCount == 1);
        }

        [Fact]
        public void CanCreateTo()
        {
            var builder = this.CreateTo(this.CreateRoute());
            Assert.True(builder.ToComponentCount == 1);
        }

        [Fact]
        public void CanChainTwoTo()
        {
            var builder = this.CreateTo(this.CreateRoute());
            builder.To("test://world?myHeader=Test");
            Assert.True(builder.ToComponentCount == 2);
        }

        [Fact]
        public void CanSetupError()
        {
            var errorComponent = Substitute.For<IErrorProcessor>();
            var builder = this.CreateTo(this.CreateRoute());
            builder.Error(errorComponent);
            Assert.True(builder.WillProcessError);
        }

        [Fact]
        public void CanSetupAtomic()
        {
            var builder = this.CreateTo(this.CreateRoute());
            builder = this.CreateAtomic(builder);
            Assert.True(builder.IsAtomic);
        }

        [Fact]
        public void RouteBuilderThrowsException()
        {
            Assert.Throws<Core.Exceptions.RouteUriException>(() => { this.CreateRoute(string.Empty); });
        }

        [Theory]
        [InlineData(TimeUnit.Minute)]
        [InlineData(TimeUnit.Hour)]
        public void CanCreateScheduleEvery(TimeUnit unit)
        {
            var builder = this.CreateTo(this.CreateRoute());
            builder.ScheduleEvery(unit);
            Assert.True(builder.IsScheduled);
        }

        [Theory]
        [InlineData(TimeUnit.Minute, 1)]
        [InlineData(TimeUnit.Hour, 1)]
        public void CanCreateScheduleAt(TimeUnit unit, int value)
        {
            var builder = this.CreateTo(this.CreateRoute());
            builder.ScheduleAt(unit, value);
            Assert.True(builder.IsScheduled);
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