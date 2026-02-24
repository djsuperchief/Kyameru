using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Kyameru.Component.Rest.Contracts;
using Kyameru.Component.Rest.Implementation;
using Kyameru.Core.Entities;
using Kyameru.Core.Enums;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Kyameru.Component.Rest.Tests.Utils;

public abstract class BaseTestWithMockHandler
{
    protected static readonly Dictionary<string, bool> Methods = new()
    {
        { "post", true },
        { "put", true },
        { "get", true },
        { "delete", true },
        { "connect", true },
        { "head", true },
        { "options", true },
        { "trace", true },
        { "patch", true },
        { "invalid", false }
    };

    protected static readonly List<string> BodyMethods = new()
    {
        "POST",
        "PUT",
        "PATCH"
    };
    
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
    
    public static IEnumerable<object[]> HttpBodyTests()
    {
        foreach (var method in BodyMethods)
        {
            yield return
            [
                "application/json",
                "{ \"Test\":\"Hello World\"}",
                new Content.JsonContent() { Test = "Hello World" },
                method,
                GetJsonResponse
            ];
            yield return
            [
                "text/json",
                "{ \"Test\":\"Hello World\"}",
                new Content.JsonContent() { Test = "Hello World" },
                method,
                GetJsonResponse
            ];
            yield return
            [
                "text/plain",
                "Hello World",
                "Hello World",
                method,
                GetStringResponse
            ];
            yield return
            [
                "application/octet-stream",
                Encoding.UTF8.GetBytes("Hello world"),
                Encoding.UTF8.GetBytes("Hello world"),
                method,
                GetByteArray
            ];
        }
    }

    private static async Task<object> GetJsonResponse(Routable routable)
    {
        var response = (routable.Body as JsonContent)!.Value as Entities.GetResponse;
        var bodyString = await response!.GetStringContentBody();
        return JsonSerializer.Deserialize<Content.JsonContent>(bodyString)!;
    }

    private static async Task<object> GetStringResponse(Routable routable)
    {
        var response = (routable.Body as JsonContent)!.Value as Entities.GetResponse;
        var bodyString = await response!.GetStringContentBody();
        return bodyString;
    }

    private static async Task<object> GetByteArray(Routable routable)
    {
        var response = (routable.Body as JsonContent)!.Value as Entities.GetResponse;
        var body = await response!.GetBodyByteArrayAsString();
        return body;
    }
    
    public static IEnumerable<object[]> MethodTests()
    {
        foreach(var method in Methods)
        {
            yield return [method.Key, method.Value];
        }
    }

    public static IEnumerable<object[]> JustMethodTests()
    {
        foreach (var method in Methods)
        {
            if (method.Value)
            {
                yield return [method.Key];
            }
        }
    }
    
    protected IServiceProvider GetServiceProvider(out Guid fromId, out Guid toId)
    {
        var serviceCollection = new ServiceCollection();
        toId = serviceCollection.RegisterKyameruDependency<IAuthStrategy>(ChainLinkDependencyType.From, () => new NoAuth()).Id;
        fromId = serviceCollection.RegisterKyameruDependency<IAuthStrategy>(ChainLinkDependencyType.To, () => new NoAuth()).Id;
        var serviceProvider = serviceCollection.BuildServiceProvider();
        // need to actually build services
        
        return serviceProvider;
    }
}