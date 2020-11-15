using Kyameru.Core.Entities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Tests.ComponentTests
{
    [TestFixture(Category = "Components")]
    public class AddHeaderTests
    {
        [Test]
        public void CanProcessString()
        {
            Kyameru.Core.BaseComponents.AddHeader header = new Core.BaseComponents.AddHeader("Processing", "String");
            Routable routable = new Routable(new Dictionary<string, string>(), "Test");
            header.Process(routable);
            Assert.AreEqual("String", routable.Headers["Processing"]);
        }

        [Test]
        public void CanProcessCallbackOne()
        {
            Kyameru.Core.BaseComponents.AddHeader header = new Core.BaseComponents.AddHeader("Processing", () =>
            {
                return "CallbackOne";
            });
            Routable routable = new Routable(new Dictionary<string, string>(), "Test");
            header.Process(routable);
            Assert.AreEqual("CallbackOne", routable.Headers["Processing"]);
        }

        [Test]
        public void CanProcessCallbackTwo()
        {
            Kyameru.Core.BaseComponents.AddHeader header = new Core.BaseComponents.AddHeader("Processing", (x) =>
            {
                return x.Headers["Target"];
            });
            Routable routable = new Routable(new Dictionary<string, string>() { { "Target", "drive" } }, "Test");
            header.Process(routable);
            Assert.AreEqual("drive", routable.Headers["Processing"]);
        }

        [Test]
        public void SetErrorWorks()
        {
            Kyameru.Core.BaseComponents.AddHeader header = new Core.BaseComponents.AddHeader("Processing", (x) =>
            {
                throw new NotImplementedException();
            });
            Routable routable = new Routable(new Dictionary<string, string>() { { "Target", "drive" } }, "Test");
            header.Process(routable);
            Assert.NotNull(routable.Error);
        }
    }
}