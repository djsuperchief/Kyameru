using System.Net.Http.Json;
using NSubstitute;

namespace Kyameru.Component.Rest.Tests.Utils;

public class BaseTestWithMockHandler
{
    protected static MockMessageHandler GetMockMessageHandler()
    {
        var httpMessageHandlerMock = Substitute.ForPartsOf<MockMessageHandler>();
        httpMessageHandlerMock.Send(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(x =>
            {
                var requestMessage = x.Arg<HttpRequestMessage>();
                object? data = null;
                if (requestMessage.Content != null)
                {
                    data = requestMessage.Content;
                }
                
                return new HttpResponseMessage()
                {
                    Content = JsonContent.Create<Entities.GetResponse>(new()
                    {
                        Method = requestMessage.Method.ToString().ToUpper(),
                        Url = requestMessage.RequestUri.ToString(),
                        Data = data
                    })
                };

                
                
            });
        
        return httpMessageHandlerMock;
    }
}