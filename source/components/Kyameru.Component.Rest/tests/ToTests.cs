using System;
using System.Net.Http.Json;
using Kyameru.Component.Rest.Implementation;
using Kyameru.Component.Rest.Tests.Utils;
using Kyameru.Core.Entities;
using NSubstitute;

namespace Kyameru.Component.Rest.Tests;

public class ToTests
{
    [Fact]
    public async Task ToExecutesGetRequest_WithoutAuth()
    {
        var httpMessageHandlerMock = GetMockHttpMessageHandler();
        var routeAttr = new RouteAttributes("rest://api/v1/hello?endpoint=localhost:8080");

        var to = new RestTo(httpMessageHandlerMock);
        to.SetHeaders(routeAttr.Headers);
        var routable = new Routable(new Dictionary<string, string>(), "test");
        await to.ProcessAsync(routable, default);

        var response = (routable.Body as JsonContent).Value as Entities.GetResponse;

        Assert.Equal("GET", response.Method);
        Assert.Equal("https://localhost:8080/api/v1/hello", response.Url);
    }

    [Fact]
    public async Task ToExecutesGetRequestWithParameters_WithoutAuth()
    {
        var httpMessageHandlerMock = GetMockHttpMessageHandler();
        var routeAttr = new RouteAttributes("rest://api/v1/hello?endpoint=localhost:8080&id=1");
        var to = new RestTo(httpMessageHandlerMock);
        to.SetHeaders(routeAttr.Headers);
        var routable = new Routable(new Dictionary<string, string>(), "test");
        await to.ProcessAsync(routable, default);

        var response = (routable.Body as JsonContent).Value as Entities.GetResponse;

        Assert.Equal("GET", response.Method);
        Assert.Equal("https://localhost:8080/api/v1/hello?id=1", response.Url);
    }

    private static MockHttpMessageHandler GetMockHttpMessageHandler()
    {
        var httpMessageHandlerMock = Substitute.ForPartsOf<MockHttpMessageHandler>();
        httpMessageHandlerMock.Send(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
        .ReturnsForAnyArgs(x =>
        {
            var requestMessage = x.Arg<HttpRequestMessage>();
            return new HttpResponseMessage()
            {
                Content = JsonContent.Create<Entities.GetResponse>(new Entities.GetResponse()
                {
                    Method = requestMessage.Method.ToString(),
                    Url = requestMessage.RequestUri.ToString()
                })
            };
        });

        return httpMessageHandlerMock;
    }
}
