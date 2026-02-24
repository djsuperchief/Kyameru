using System.Net.Http.Json;
using Kyameru.Component.Rest.Messages;
using Kyameru.Component.Rest.Tests.Utils;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Kyameru.Core.Enums;
using Kyameru.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Kyameru.Component.Rest.Tests;

public class AuthTests : BaseTestWithMockHandler
{
    [Theory]
    [MemberData(nameof(AuthTestData))]
    public async Task HttpGetWithAuthHasCorrectHeaders(Func<IServiceCollection, (ChainLinkDependency fromToken, ChainLinkDependency toToken)> authRegister, Func<HttpRequestMessage, string> extract)
    {
        var httpMessageHandlerMock = Substitute.ForPartsOf<MockMessageHandler>();
        var receivedFromToken = string.Empty;
        var receivedToToken = string.Empty;
        var testThread = TestThread.CreateDeferred(20);
        httpMessageHandlerMock.Send(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(x =>
            {
                var requestMessage = x.Arg<HttpRequestMessage>();
                object? data = null;
                if (requestMessage.Content != null)
                {
                    data = requestMessage.Content;
                }

                if (requestMessage.RequestUri.ToString().Contains("/from"))
                {
                    receivedFromToken = extract(requestMessage);
                }
                else
                {
                    receivedToToken = extract(requestMessage);
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

        var servicesCollection = GetServiceCollection();
        servicesCollection.AddTransient<HttpMessageHandler>((x) => httpMessageHandlerMock);
        servicesCollection.AddSingleton(httpMessageHandlerMock);
        var tokens = authRegister(servicesCollection);
        var routeId = Route.From("rest://localhost:8080/api/v1/from/hello")
            .To("rest://localhost:8080/api/v1/to/hello")
            .AddDependency(tokens.fromToken, ChainLinkDependencyType.From)
            .AddDependency(tokens.toToken, ChainLinkDependencyType.To)
            .EventTrigger()
            .Build(servicesCollection);
        
        var serviceProvider = servicesCollection.BuildServiceProvider();
        var service = serviceProvider.GetService<IHostedService>();
        var exchange = serviceProvider.GetRequiredService<IKExchange>();
        
        var message = HttpMessageData.Create("Test");
        testThread.SetThread(service.StartAsync);
        testThread.Start();
        await exchange.PublishMessageAsync(routeId, message, testThread.CancelToken);
        testThread.WaitForExecution();
        Assert.Equal("FromApiKey",  receivedFromToken);
        Assert.Equal("ToApiKey", receivedToToken);
    }

    public static IEnumerable<object[]> AuthTestData()
    {
        yield return [GetApiAuth, GetApiAuthToken];
    }

    private static (ChainLinkDependency from, ChainLinkDependency to) GetApiAuth(IServiceCollection services)
    {
        var from = services.RegisterKyameruRestAuthApi("FromApiKey");
        var to =  services.RegisterKyameruRestAuthApi("ToApiKey");
        return (from, to);
    }

    private static string GetApiAuthToken(HttpRequestMessage requestMessage)
    {
        return requestMessage.Headers.GetValues("X-API-KEY").FirstOrDefault();
    }
    
    
    private IServiceCollection GetServiceCollection()
    {
        var logger = Substitute.For<ILogger<Route>>();
        var routerLogger = Substitute.For<ILogger<IKRouter>>();
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ILogger<Kyameru.Route>>(sp => logger);
        serviceCollection.AddTransient<ILogger<IKRouter>>(sp => routerLogger);

        // Any generic items here
        return serviceCollection;
    }
}