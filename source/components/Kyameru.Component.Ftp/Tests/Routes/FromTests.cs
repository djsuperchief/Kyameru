using Kyameru.Component.Ftp.Contracts;
using Kyameru.Component.Ftp.Settings;
using Kyameru.Core.Entities;
using Moq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core;
using Kyameru.Core.Sys;
using Xunit;

namespace Kyameru.Component.Ftp.Tests.Routes
{
    public class FromTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task FromDownloadsAndDeletes(bool deletes)
        {
            var tokenSource = new CancellationTokenSource();
            Mock<IWebRequestUtility> webRequestFactory = this.GetWebRequest();
            AutoResetEvent autoReset = new AutoResetEvent(false);
            Times times = Times.Never();
            if (deletes)
            {
                times = Times.Once();
            }

            webRequestFactory.Setup(x => x.DeleteFile(It.IsAny<FtpSettings>(), "Test.txt", It.IsAny<bool>(), It.IsAny<CancellationToken>())).Returns(async (FtpSettings x, string y, bool z, CancellationToken c) =>
            {
                await Task.CompletedTask;
            });
            From from = new From(this.GetRoute(deletes).Headers, webRequestFactory.Object);
            Routable routable = null;

            from.OnActionAsync += delegate (object sender, RoutableEventData e)
            {
                routable = e.Data;
                routable.SetHeader("&Method", "ASYNC");
                autoReset.Set();
                return Task.CompletedTask;
            };
            from.Setup();
            await from.StartAsync(tokenSource.Token);

            // possible the crash is being caused by 
            if (routable == null)
            {
                autoReset.WaitOne(10000);
            }

            Assert.Equal("Hello ftp", Encoding.UTF8.GetString((byte[])routable.Body));
            webRequestFactory.Verify(x => x.DeleteFile(It.IsAny<FtpSettings>(), "Test.txt", It.IsAny<bool>(), It.IsAny<CancellationToken>()), times);

            await from.StopAsync(tokenSource.Token);
            Assert.Equal("ASYNC", routable.Headers["Method"]);

            Assert.False(from.PollerIsActive);

        }

        [Fact]
        public void WebRequestLogIsHandled()
        {
            bool hasLogged = false;
            Mock<IWebRequestUtility> webRequestFactory = this.GetWebRequest();
            From from = new From(this.GetRoute(false).Headers, webRequestFactory.Object);
            from.Setup();
            from.OnLog += (object sender, Log e) =>
            {
                hasLogged = true;
            };

            webRequestFactory.Raise(x => x.OnLog += null, this, "test");
            Assert.True(hasLogged);
        }

        public Mock<IWebRequestUtility> GetWebRequest()
        {
            Mock<IWebRequestUtility> response = new Mock<IWebRequestUtility>();
            response.Setup(x => x.DownloadFile("Test.txt", It.IsAny<FtpSettings>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(Encoding.UTF8.GetBytes("Hello ftp")));
            response.Setup(x => x.GetDirectoryContents(It.IsAny<FtpSettings>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new List<string>() { "Test.txt" }));
            return response;
        }

        private RouteAttributes GetRoute(bool delete)
        {
            return new RouteAttributes($"ftp://test:banana@127.0.0.1/out&Delete={delete}&PollTime=5000");
        }
    }
}