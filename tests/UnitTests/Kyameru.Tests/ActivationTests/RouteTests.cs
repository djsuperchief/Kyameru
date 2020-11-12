using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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
            Assert.DoesNotThrow(() =>
            {
                Route.From("test:///hello");
            });
        }
    }
}