using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kyameru.Core.Entities;
using Kyameru.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kyameru.Tests.EntityTests;

public class RoutableTests : BaseTests
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
    [InlineData(true)]
    [InlineData(false)]
    public async Task ProcessExitWorks(bool earlyExit)
    {
        var services = GetServiceDescriptors();
        var processComponent = Substitute.For<IProcessor>();
        var thread = TestThread.CreateDeferred();
        Routable routable = null;
        processComponent.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            if (earlyExit)
            {
                x.Arg<Routable>().SetExitRoute("Manually triggered exit");
                routable = x.Arg<Routable>();
            }

            return Task.CompletedTask;
        });

        var generics = Component.Generic.Builder.Create()
            .WithFrom()
            .WithTo(x =>
            {
                x.SetHeader("TO", "Executed");
                routable = x;
                thread.Continue();
            });

        var builder = Route.From("generic:///nope")
            .Process(processComponent)
            .To("generic:///nope");

        var service = BuildAndGetServices(builder, generics);
        thread.SetThread(service.StartAsync);
        thread.StartAndWait();
        await thread.CancelAsync();

        Assert.Equal(!earlyExit, routable.Headers.ContainsKey("TO"));
    }

    public static IEnumerable<object[]> BodyTestCases()
    {
        yield return new object[] { new BodyTests<string>("test", "String") };
        yield return new object[] { new BodyTests<int>(1, "Int32") };
    }

    private Routable CreateRoutableMessage()
    {
        return new Routable(new System.Collections.Generic.Dictionary<string, string>()
        {
            {"&Test", "value" }
        }, "test");
    }
}