using System;
using System.Net.Http.Json;
using NSubstitute;

namespace Kyameru.Component.Rest.Tests.Utils;

public abstract class BaseTestWithMockhandler
{
    protected static MockHttpMessageHandler GetMockHttpMessageHandler()
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
                    Method = requestMessage.Method.ToString().ToUpper(),
                    Url = requestMessage.RequestUri.ToString()
                })
            };
        });

        return httpMessageHandlerMock;
    }
}
