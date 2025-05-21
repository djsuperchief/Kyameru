using Kyameru.Core.Entities;
using System;
using System.Collections.Generic;
using Xunit;

namespace Kyameru.Tests.ComponentTests;

public class AddHeaderTests
{
    [Fact]
    public void CanProcessString()
    {
        var header = new Core.BaseProcessors.AddHeader("Processing", "String");
        var routable = new Routable(new Dictionary<string, string>(), "Test");
        header.Process(routable);
        Assert.Equal("String", routable.Headers["Processing"]);
    }

    [Fact]
    public void CanProcessCallbackOne()
    {
        var header = new Core.BaseProcessors.AddHeader("Processing", () =>
        {
            return "CallbackOne";
        });
        var routable = new Routable(new Dictionary<string, string>(), "Test");
        header.Process(routable);
        Assert.Equal("CallbackOne", routable.Headers["Processing"]);
    }

    [Fact]
    public void CanProcessCallbackTwo()
    {
        var header = new Core.BaseProcessors.AddHeader("Processing", (x) =>
        {
            return x.Headers["Target"];
        });
        var routable = new Routable(new Dictionary<string, string>() { { "Target", "drive" } }, "Test");
        header.Process(routable);
        Assert.Equal("drive", routable.Headers["Processing"]);
    }

    [Fact]
    public void SetErrorWorks()
    {
        var header = new Core.BaseProcessors.AddHeader("Processing", (x) =>
        {
            throw new NotImplementedException();
        });
        var routable = new Routable(new Dictionary<string, string>() { { "Target", "drive" } }, "Test");
        header.Process(routable);
        Assert.NotNull(routable.Error);
    }
}