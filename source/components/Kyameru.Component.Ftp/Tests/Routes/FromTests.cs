using Kyameru.Component.Ftp.Contracts;
using Kyameru.Component.Ftp.Settings;
using Kyameru.Core.Entities;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Sys;
using Xunit;
using System;
using NSubstitute;

namespace Kyameru.Component.Ftp.Tests.Routes
{
    public class FromTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task FromDownloadsAndDeletes(bool deletes)
        {
            var timespanSeconds = 6;
            var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timespanSeconds));
            var webRequestFactory = this.GetWebRequest();
            webRequestFactory.ClearReceivedCalls();
            var autoReset = new AutoResetEvent(false);
            var times = 0;
            if (deletes)
            {
                times = 1;
            }
            webRequestFactory.DeleteFile(default, "Test.txt", default, default).Returns(x =>
            {
                return Task.CompletedTask;
            });

            var from = new From(this.GetRoute(deletes).Headers, webRequestFactory);
            Routable routable = null;

            from.Setup();
            from.OnActionAsync += delegate (object sender, RoutableEventData e)
            {
                routable = e.Data;
                routable.SetHeader("&Method", "ASYNC");
                return Task.CompletedTask;
            };

            var thread = new Thread(async () =>
            {
                await from.StartAsync(tokenSource.Token);
            });

            thread.Start();
            autoReset.WaitOne(TimeSpan.FromSeconds(timespanSeconds));
            tokenSource.Cancel();

            // possible the crash is being caused by 

            Assert.Equal("Hello ftp", Encoding.UTF8.GetString((byte[])routable.Body));
            if (deletes)
            {
                await webRequestFactory.Received().DeleteFile(Arg.Any<FtpSettings>(), "Test.txt", Arg.Any<bool>(), Arg.Any<CancellationToken>());
            }
            else
            {
                await webRequestFactory.DidNotReceive().DeleteFile(Arg.Any<FtpSettings>(), "Test.txt", Arg.Any<bool>(), Arg.Any<CancellationToken>());
            }


            await from.StopAsync(tokenSource.Token);
            Assert.Equal("ASYNC", routable.Headers["Method"]);

            Assert.False(from.PollerIsActive);

        }

        [Fact]
        public void WebRequestLogIsHandled()
        {
            bool hasLogged = false;
            var webRequestFactory = this.GetWebRequest();
            From from = new From(this.GetRoute(false).Headers, webRequestFactory);
            from.Setup();
            from.OnLog += (object sender, Log e) =>
            {
                hasLogged = true;
            };
            webRequestFactory.OnLog += Raise.Event<EventHandler<string>>(this, "test");

            Assert.True(hasLogged);
        }

        public IWebRequestUtility GetWebRequest()
        {
            var response = Substitute.For<IWebRequestUtility>();
            response.DownloadFile("Test.txt", Arg.Any<FtpSettings>(), Arg.Any<CancellationToken>()).ReturnsForAnyArgs(x =>
            {
                return Task.FromResult(Encoding.UTF8.GetBytes("Hello ftp"));
            });

            response.GetDirectoryContents(default, default).ReturnsForAnyArgs(x =>
            {
                return Task.FromResult(new List<string>() { "Test.txt" });
            });

            return response;
        }

        private RouteAttributes GetRoute(bool delete)
        {
            return new RouteAttributes($"ftp://test:banana@127.0.0.1/out&Delete={delete}&PollTime=5000");
        }
    }
}