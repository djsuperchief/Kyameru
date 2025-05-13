using System.Collections.Generic;
using System.Threading.Tasks;
using Kyameru.Core.Entities;
using Kyameru.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kyameru.Tests.EntityTests;

public class RoutableTests
{
    private readonly ILogger<Route> logger = Substitute.For<ILogger<Route>>();

    [Fact]
    public void CreatedHeaderError()
    {
        var routable = this.CreateRoutableMessage();
        Assert.Throws<Kyameru.Core.Exceptions.CoreException>(() => routable.SetHeader("Test", "changed"));
    }

    [Fact]
    public void UserImmutableThrowsHeader()
    {
        var routable = this.CreateRoutableMessage();
        routable.SetHeader("&Nope", "Nope");
        Assert.Throws<Kyameru.Core.Exceptions.CoreException>(() => routable.SetHeader("Nope", "yep"));
    }

    [Fact]
    public void UserMutableWorks()
    {
        var routable = this.CreateRoutableMessage();
        routable.SetHeader("FileType", "txt");
        routable.SetHeader("FileType", "jpg");
        Assert.Equal("jpg", routable.Headers["FileType"]);
    }

    [Fact]
    public void SetBodyWorks()
    {
        var body = "body text";
        var routable = this.CreateRoutableMessage();
        routable.SetBody<string>(body);
        Assert.Equal(body, routable.Body);
    }

    [Theory]
    [MemberData(nameof(BodyTestCases))]
    public void SetBodyWorksWithHeader(IBodyTests bodyTest)
    {
        Assert.True(bodyTest.IsEqual(this.CreateRoutableMessage()));
    }

    [Theory]
    [InlineData("TO", true)]
    [InlineData("ATOMIC", false)]
    public async Task ProcessExitWorks(string call, bool setupComponent)
    {
        var testName = $"ProcessExitWorks_{call}";
        var processComponent = Substitute.For<IProcessor>();
        processComponent.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            x.Arg<Routable>().SetHeader("SetExit", "true");
            if (setupComponent)
            {
                x.Arg<Routable>().SetExitRoute("Manually triggered exit");
            }

            return Task.CompletedTask;
        });

        Assert.False(await this.RunProcess(call, testName, processComponent));
    }

    public static IEnumerable<object[]> BodyTestCases()
    {
        yield return new object[] { new BodyTests<string>("test", "String") };
        yield return new object[] { new BodyTests<int>(1, "Int32") };
    }

    private async Task<bool> RunProcess(
        string callsContain,
        string test,
        IProcessor processComponent)
    {
        var service = this.GetRoute(test, processComponent);
        var thread = TestThread.CreateNew(service.StartAsync, 3);
        thread.StartAndWait();
        await thread.CancelAsync();

        return Kyameru.Component.Test.GlobalCalls.CallDict[test].Contains(callsContain);
    }

    private Routable CreateRoutableMessage()
    {
        return new Routable(new System.Collections.Generic.Dictionary<string, string>()
        {
            {"&Test", "value" }
        }, "test");
    }

    private IHostedService GetRoute(string test, IProcessor component)
    {
        var serviceCollection = this.GetServiceDescriptors();
        Kyameru.Route.From($"test://hello?TestName={test}")
            .Process(component)
            .To("test://world")
            .Build(serviceCollection);
        var provider = serviceCollection.BuildServiceProvider();
        return provider.GetService<IHostedService>();
    }

    private IServiceCollection GetServiceDescriptors()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ILogger<Kyameru.Route>>(sp =>
        {
            return this.logger;
        });

        return serviceCollection;
    }
}