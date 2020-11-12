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
            var route = this.CreateRoute();
            route.AddHeader("Test", "Test");
            Assert.IsTrue(route.ComponentCount > 0);
        }

        private Core.RouteBuilder CreateRoute()
        {
            return Route.From("test://hello");
        }
    }
}