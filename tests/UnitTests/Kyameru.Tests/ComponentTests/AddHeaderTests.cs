using Kyameru.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Kyameru.Tests.ComponentTests
{
    public class AddHeaderTests
    {
        [Fact]
        public void CanProcessString()
        {
            Kyameru.Core.BaseComponents.AddHeader header = new Core.BaseComponents.AddHeader("Processing", "String");
            Routable routable = new Routable(new Dictionary<string, string>(), "Test");
            header.Process(routable);
            Assert.Equal("String", routable.Headers["Processing"]);
        }

        [Fact]
        public void CanProcessCallbackOne()
        {
            Kyameru.Core.BaseComponents.AddHeader header = new Core.BaseComponents.AddHeader("Processing", () =>
            {
                return "CallbackOne";
            });
            Routable routable = new Routable(new Dictionary<string, string>(), "Test");
            header.Process(routable);
            Assert.Equal("CallbackOne", routable.Headers["Processing"]);
        }

        [Fact]
        public void CanProcessCallbackTwo()
        {
            Kyameru.Core.BaseComponents.AddHeader header = new Core.BaseComponents.AddHeader("Processing", (x) =>
            {
                return x.Headers["Target"];
            });
            Routable routable = new Routable(new Dictionary<string, string>() { { "Target", "drive" } }, "Test");
            header.Process(routable);
            Assert.Equal("drive", routable.Headers["Processing"]);
        }

        [Fact]
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