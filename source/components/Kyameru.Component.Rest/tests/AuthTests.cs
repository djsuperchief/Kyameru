using System.Net.Http.Json;
using System.Text;
using Kyameru.Component.Rest.Contracts;
using Kyameru.Component.Rest.Messages;
using Kyameru.Component.Rest.Tests.Utils;
using Kyameru.Core.Comms;
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
    public async Task HttpGetWithAuthHasCorrectHeaders(Func<IServiceCollection, (ChainLinkDependency fromToken, ChainLinkDependency toToken)> authRegister, Func<HttpRequestMessage, string> extract, string expectedFrom, string expectedTo)
    {
        var httpMessageHandlerMock = Substitute.ForPartsOf<MockMessageHandler>();
        var receivedFromToken = string.Empty;
        var receivedToToken = string.Empty;
        var testThread = TestThread.CreateDeferred(5);
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
        Assert.Equal(expectedFrom,  receivedFromToken);
        Assert.Equal(expectedTo, receivedToToken);
    }

    public static IEnumerable<object[]> AuthTestData()
    {
        yield return [GetApiAuth, GetApiAuthToken, "FromApiKey", "ToApiKey"];
        yield return [GetBearerAuth, GetBearerAuthToken, "Bearer FROMTOKEN", "Bearer TOTOKEN" ];
        yield return [GetBasicAuth, GetBasicAuthToken, "username:passwordfrom", "username:passwordto" ];
        yield return [GetCustomAuth, GetCustomAuthToken, "CustomFrom", "CustomTo"];
    }

    private static (ChainLinkDependency fromToken, ChainLinkDependency toToken) GetCustomAuth(
        IServiceCollection services)
    {
        var from = services.RegisterKyameruDependency<IAuthStrategy>(ChainLinkDependencyType.From,() => new CustomAuth("CustomFrom"));
        var to = services.RegisterKyameruDependency<IAuthStrategy>(ChainLinkDependencyType.To,() => new CustomAuth("CustomTo"));
        
        return (from, to);
    }

    private static string GetCustomAuthToken(HttpRequestMessage requestMessage)
    {
        return requestMessage.Headers.GetValues("CustomAuth").FirstOrDefault();
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

    private static (ChainLinkDependency fromToken, ChainLinkDependency toToken) GetBearerAuth(
        IServiceCollection services)
    {
        var from = services.RegisterKyameruRestAuthBearer("FROMTOKEN");
        var to = services.RegisterKyameruRestAuthBearer("TOTOKEN");
        return (from, to);
    }

    private static string GetBearerAuthToken(HttpRequestMessage requestMessage)
    {
        return requestMessage.Headers.Authorization.ToString();
    }

    private static (ChainLinkDependency fromToken, ChainLinkDependency toToken) GetBasicAuth(
        IServiceCollection services)
    {
        var from = services.RegisterKyameruRestAuthBasic("username", "passwordfrom");
        var to = services.RegisterKyameruRestAuthBasic("username", "passwordto");
        return (from, to);
    }

    private static string GetBasicAuthToken(HttpRequestMessage requestMessage)
    {
        var token = requestMessage.Headers.Authorization.Parameter;
        return Encoding.ASCII.GetString(Convert.FromBase64String(token));
    }
    
    private IServiceCollection GetServiceCollection()
    {
        var logger = Substitute.For<ILogger<Route>>();
        var routerLogger = Substitute.For<ILogger<KRouter>>();
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ILogger<Kyameru.Route>>(sp => logger);
        serviceCollection.AddTransient<ILogger<KRouter>>(sp => routerLogger);

        // Any generic items here
        return serviceCollection;
    }
}