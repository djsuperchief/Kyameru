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
        [Test]
        public void CanCreateFrom()
        {
            Assert.IsTrue(this.CreateRoute().FromValid);
        }

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
        public void RouteBuilderThrowsException()
        {
            Assert.Throws<Core.Exceptions.ActivationException>(() => { Core.RouteBuilder route = this.CreateRoute("invalid"); });
        }

        private Core.RouteBuilder CreateRoute(string route = "test://hello")
        {
            return Route.From(route);
        }
    }
}