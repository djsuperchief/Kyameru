using Kyameru.Core.Entities;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Kyameru.Component.Slack.Tests;

public class ToTests
{
    [Fact]
    public async Task CanSendMessageFromBodyAsync()
    {
        var slackTo = this.GetComponent("Body", this.SetupOkHandler());
        var routable = new Routable(new Dictionary<string, string>(), "This is a slack message");
        await slackTo.ProcessAsync(routable, default);
        Assert.Null(routable.Error);
    }

    [Fact]
    public async Task SendMessageInError()
    {
        var slackTo = this.GetComponent("Body", this.SetupErrorHandler());
        var routable = new Routable(new Dictionary<string, string>(), "This is a slack message");
        await slackTo.ProcessAsync(routable, default);
        Assert.NotNull(routable.Error);
    }

    [Fact]
    public async Task CanSendMessageFromHeader()
    {
        var headers = new Dictionary<string, string>()
        {
            { "SlackMessage", "test" }
        };
        var slackTo = this.GetComponent("Header", this.SetupOkHandler());
        var routable = new Routable(headers, "This is a slack message");
        await slackTo.ProcessAsync(routable, default);
        Assert.Null(routable.Error);
    }

    [Fact]
    public async Task LoggingIsTriggered()
    {
        var callbackMade = false;
        var slackTo = this.GetComponent("Body", this.SetupOkHandler());
        var routable = new Routable(new Dictionary<string, string>(), "This is a slack message");
        slackTo.OnLog += delegate (object sender, Log log)
        {
            callbackMade = true;
        };
        await slackTo.ProcessAsync(routable, default);
        Assert.True(callbackMade);

    }


    [Fact]
    public async Task BlankHeaderErrors()
    {
        var slackTo = this.GetComponent("Header", this.SetupOkHandler());
        var routable = new Routable(new Dictionary<string, string>(), "This is a slack message");
        await slackTo.ProcessAsync(routable, default);
        Assert.NotNull(routable.Error);
    }

    private SlackTo GetComponent(string source, HttpMessageHandler messageHandler)
    {
        return new SlackTo(this.GetHeaders(source), messageHandler);
    }

    private HttpMessageHandler SetupOkHandler()
    {
        return MockHttpMessageHandler.Create("Ok", System.Net.HttpStatusCode.OK);
    }

    private HttpMessageHandler SetupErrorHandler()
    {
        return MockHttpMessageHandler.Create("Slack not here", System.Net.HttpStatusCode.BadRequest);
    }

    private Dictionary<string, string> GetHeaders(string source)
    {
        return new Dictionary<string, string>()
        {
            { "Target", "test" },
            { "MessageSource", source },
            { "Username", "Kyameru" },
            { "Channel", "General" }
        };
    }
}