using Kyameru.Component.Ftp.Contracts;
using Kyameru.Component.Ftp.Settings;
using Kyameru.Core.Entities;
using Moq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core;
using Xunit;

namespace Kyameru.Component.Ftp.Tests.Routes
{
    public class FromTests
    {
        [Theory]
        [InlineData(true, false)]
        [InlineData(false, false)]
        [InlineData(true, true)]
        [InlineData(false, true)]
        public async Task FromDownloadsAndDeletes(bool deletes, bool isAsync)
        {
            var tokenSource = new CancellationTokenSource();
            Mock<IWebRequestUtility> webRequestFactory = this.GetWebRequest();
            AutoResetEvent autoReset = new AutoResetEvent(false);
            Times times = Times.Never();
            if (deletes)
            {
                times = Times.Once();
            }

            webRequestFactory.Setup(x => x.DeleteFile(It.IsAny<FtpSettings>(), "Test.txt", It.IsAny<bool>())).Verifiable();
            From from = new From(this.GetRoute(deletes).Headers, webRequestFactory.Object);

            Routable routable = null;
            from.OnAction += delegate (object sender, Routable e)
            {
                routable = e;
                autoReset.Set();
            };
            
            from.OnActionAsync += delegate (object sender, RoutableEventData e)
            {
                routable = e.Data;
                routable.SetHeader("&Method", "ASYNC");
                autoReset.Set();
                return Task.CompletedTask;
            };
            from.Setup();
            if (isAsync)
            {
                await from.StartAsync(tokenSource.Token);
            }
            else
            {
                from.Start();
            }
            

            autoReset.WaitOne(60000);

            Assert.Equal("Hello ftp", Encoding.UTF8.GetString((byte[])routable.Body));
            webRequestFactory.Verify(x => x.DeleteFile(It.IsAny<FtpSettings>(), "Test.txt", It.IsAny<bool>()), times);
            if (isAsync)
            {
                await from.StopAsync(tokenSource.Token);
                Assert.Equal("ASYNC", routable.Headers["Method"]);
            }
            else
            {
                from.Stop();    
            }
            
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
            response.Setup(x => x.DownloadFile("Test.txt", It.IsAny<FtpSettings>())).Returns(Encoding.UTF8.GetBytes("Hello ftp"));
            response.Setup(x => x.GetDirectoryContents(It.IsAny<FtpSettings>())).Returns(new List<string>() { "Test.txt" });
            return response;
        }

        private RouteAttributes GetRoute(bool delete)
        {
            return new RouteAttributes($"ftp://test:banana@127.0.0.1/out&Delete={delete}&PollTime=5000");
        }
    }
}