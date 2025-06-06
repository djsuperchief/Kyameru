﻿using Kyameru.Component.Ftp.Contracts;
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
using Kyameru.TestUtilities;

namespace Kyameru.Component.Ftp.Tests.Routes
{
    public class FromTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task FromDownloadsAndDeletes(bool deletes)
        {
            var thread = TestThread.CreateDeferred();
            var webRequestFactory = this.GetWebRequest();
            webRequestFactory.ClearReceivedCalls();
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
                thread.Continue();
                return Task.CompletedTask;
            };

            thread.SetThread(from.StartAsync);
            thread.StartAndWait();
            await from.StopAsync(thread.CancelToken);
            await thread.CancelAsync();

            Assert.Equal("Hello ftp", Encoding.UTF8.GetString((byte[])routable.Body));
            if (deletes)
            {
                await webRequestFactory.Received().DeleteFile(Arg.Any<FtpSettings>(), "Test.txt", Arg.Any<bool>(), Arg.Any<CancellationToken>());
            }
            else
            {
                await webRequestFactory.DidNotReceive().DeleteFile(Arg.Any<FtpSettings>(), "Test.txt", Arg.Any<bool>(), Arg.Any<CancellationToken>());
            }

            Assert.Equal("ASYNC", routable.Headers["Method"]);
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