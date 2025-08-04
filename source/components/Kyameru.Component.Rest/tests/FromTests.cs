using System;
using System.Net.Http.Json;
using Kyameru.Component.Rest.Implementation;
using Kyameru.Component.Rest.Tests.Utils;
using Kyameru.Core.Entities;
using Kyameru.Core.Sys;
using Kyameru.TestUtilities;

namespace Kyameru.Component.Rest.Tests;

public class FromTests : BaseTestWithMockhandler
{
    [Fact]
    public async Task FromExecutesGetRequestWithParams_WithoutAuth()
    {
        var httpMessageHandlerMock = GetMockHttpMessageHandler();
        var routeAttr = new RouteAttributes("rest://api/v1/hello?endpoint=localhost:8080&id=546");
        var from = new RestFrom(httpMessageHandlerMock);
        from.SetHeaders(routeAttr.Headers);
        var routable = new Routable(new Dictionary<string, string>(), "test");
        var thread = TestThread.CreateNew(from.StartAsync, 5);
        from.OnActionAsync += async delegate(object sender, RoutableEventData e)
        {
            routable = e.Data;
            await Task.CompletedTask;
        };
        thread.StartAndWait();
        await thread.CancelAsync();
        
        var response = (routable.Body as JsonContent).Value as Entities.GetResponse;
        Assert.Equal("GET", response.Method);
        Assert.Equal("https://localhost:8080/api/v1/hello?id=546", response.Url);
    }

    [Theory]
    [InlineData("GET")]
    [InlineData("POST")]
    [InlineData("PUT")]
    [InlineData("DELETE")]
    [InlineData("PATCH")]
    [InlineData("HEAD")]
    [InlineData("OPTIONS")]
    [InlineData("TRACE")]
    [InlineData("CONNECT")]
    public async Task FromExecutesRequestWithMethod_WithoutAuth(string method)
    {
        var httpMessageHandlerMock = GetMockHttpMessageHandler();
        var routeAttr = new RouteAttributes($"rest://api/v1/hello?id=546&endpoint=localhost:8080&method={method}");
        var from = new RestFrom(httpMessageHandlerMock);
        from.SetHeaders(routeAttr.Headers);
        var routable = new Routable(new Dictionary<string, string>(), "test");
        var thread = TestThread.CreateNew(from.StartAsync, 5);
        from.OnActionAsync += async delegate(object sender, RoutableEventData e)
        {
            routable = e.Data;
            await Task.CompletedTask;
        };
        thread.StartAndWait();
        await thread.CancelAsync();
        
        var response = (routable.Body as JsonContent).Value as Entities.GetResponse;
        Assert.Equal(method, response.Method);
        Assert.Equal("https://localhost:8080/api/v1/hello?id=546", response.Url);
    }
}
