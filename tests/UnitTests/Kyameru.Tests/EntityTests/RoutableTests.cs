using System;
using Kyameru.Core.Entities;
using NUnit.Framework;

namespace Kyameru.Tests.EntityTests
{
    [TestFixture]
    public class RoutableTests
    {
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

        private Routable CreateMessage()
        {
            return new Routable(new System.Collections.Generic.Dictionary<string, string>()
            {
                {"Test", "value" }
            }, "test");
        }
    }
}