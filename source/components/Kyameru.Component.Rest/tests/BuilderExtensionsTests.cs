using System;
using Microsoft.Extensions.DependencyInjection;
using Kyameru.Core;
using Kyameru.Component.Rest.Tests.Utils;
using NSubstitute;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace Kyameru.Component.Rest.Tests;

public class BuilderExtensionsTests
{
    [Fact]
    public void CanAddAPITokenToFrom_NoHeader()
    {
        var serviceCollection = new ServiceCollection();
        Route.From("rest://api/v1/hello?endpoint=localhost:8080")
            .AuthWithApiToken("mytoken")
            .To("rest://api/v1/hello?endpoint=localhost:8080")
            .Build(serviceCollection);



    }

    private static MockHttpMessageHandler GetMockHttpMessageHandler()
    {
        var httpMessageHandlerMock = Substitute.ForPartsOf<MockHttpMessageHandler>();
        httpMessageHandlerMock.Send(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
        .ReturnsForAnyArgs(x =>
        {
            var requestMessage = x.Arg<HttpRequestMessage>();

            var response = new HttpResponseMessage()
            {
                Content = JsonContent.Create<Entities.GetResponse>(new Entities.GetResponse()
                {
                    Method = requestMessage.Method.ToString(),
                    Url = requestMessage.RequestUri!.ToString(),
                })
            };

            if (requestMessage.Headers.Contains("Auth"))
            {
                response.Headers.Add("Auth", requestMessage.Headers.Authorization!.ToString());
            }

            return response;
        });

        return httpMessageHandlerMock;
    }

    // private IServiceCollection GetServiceDescriptors()
    // {

    //     return new ServiceCollection();
    // }
}
