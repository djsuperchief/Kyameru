using Kyameru.Component.Ftp.Contracts;
using Kyameru.Component.Ftp.Settings;
using Kyameru.Core.Entities;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Kyameru.Component.Ftp.Tests.Routes
{
    [TestFixture(Category = "Routes")]
    public class FromTests
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void FromDownloadsAndDeletes(bool deletes)
        {
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
            from.Setup();
            from.Start();

            autoReset.WaitOne(60000);

            Assert.AreEqual("Hello ftp", Encoding.UTF8.GetString((byte[])routable.Body));
            webRequestFactory.Verify(x => x.DeleteFile(It.IsAny<FtpSettings>(), "Test.txt", It.IsAny<bool>()), times);
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
            return new RouteAttributes($"ftp://test@127.0.0.1/out&Delete={delete}&PollTime=5000");
        }
    }
}