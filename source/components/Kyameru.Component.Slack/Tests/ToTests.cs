using Kyameru.Core.Entities;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Kyameru.Component.Slack.Tests
{
    public class ToTests
    {
        [Fact]
        public async Task CanSendMessageFromBodyAsync()
        {
            SlackTo slackTo = this.GetComponent("Body", this.SetupOkHandler());
            Routable routable = new Routable(new Dictionary<string, string>(), "This is a slack message");
            await slackTo.ProcessAsync(routable, default);
            Assert.Null(routable.Error);
        }

        [Fact]
        public async Task SendMessageInError()
        {
            SlackTo slackTo = this.GetComponent("Body", this.SetupErrorHandler());
            Routable routable = new Routable(new Dictionary<string, string>(), "This is a slack message");
            await slackTo.ProcessAsync(routable, default);
            Assert.NotNull(routable.Error);
        }

        [Fact]
        public async Task CanSendMessageFromHeader()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                { "SlackMessage", "test" }
            };
            SlackTo slackTo = this.GetComponent("Header", this.SetupOkHandler());
            Routable routable = new Routable(headers, "This is a slack message");
            await slackTo.ProcessAsync(routable, default);
            Assert.Null(routable.Error);
        }

        [Fact]
        public async Task LoggingIsTriggered()
        {
            bool callbackMade = false;
            SlackTo slackTo = this.GetComponent("Body", this.SetupOkHandler());
            Routable routable = new Routable(new Dictionary<string, string>(), "This is a slack message");
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
            SlackTo slackTo = this.GetComponent("Header", this.SetupOkHandler());
            Routable routable = new Routable(new Dictionary<string, string>(), "This is a slack message");
            await slackTo.ProcessAsync(routable, default);
            Assert.NotNull(routable.Error);
        }

        private SlackTo GetComponent(string source, HttpMessageHandler messageHandler)
        {
            return new SlackTo(this.GetHeaders(source), messageHandler);
        }

        private HttpMessageHandler SetupOkHandler()
        {
            Mock<HttpMessageHandler> messageHandler = new Mock<HttpMessageHandler>();
            messageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage()
                {
                    Content = new StringContent("Ok"),
                    StatusCode = System.Net.HttpStatusCode.OK
                }));

            return messageHandler.Object;
        }

        private HttpMessageHandler SetupErrorHandler()
        {
            Mock<HttpMessageHandler> messageHandler = new Mock<HttpMessageHandler>();
            messageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage()
                {
                    Content = new StringContent("Slack not here"),
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }));

            return messageHandler.Object;
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
}