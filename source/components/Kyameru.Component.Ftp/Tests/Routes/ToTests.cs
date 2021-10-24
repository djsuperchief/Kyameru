using Kyameru.Component.Ftp.Contracts;
using Kyameru.Component.Ftp.Settings;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kyameru.Component.Ftp.Tests.Routes
{
    [TestFixture(Category = "Routes")]
    public class ToTests
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CanUploadFile(bool stringBody)
        {
            Mock<IWebRequestUtility> webRequestUtility = this.GetWebRequest();
            To to = new To(this.GetRoute().Headers, webRequestUtility.Object);
            Routable routable = new Routable(new Dictionary<string, string>()
            {
                { "SourceFile", "Test.txt" }
            },
            Encoding.UTF8.GetBytes("Hello")
            );
            if(stringBody)
            {
                routable.SetBody<string>("Hello");
            }

            to.Process(routable);
            webRequestUtility.VerifyAll();
        }

        [Test]
        public void CanUploadAndArchive()
        {
            Mock<IWebRequestUtility> webRequestUtility = this.GetWebRequest();
            To to = new To(this.GetRoute(true, "File").Headers, webRequestUtility.Object);
            Routable routable = this.WriteFile();
            to.Process(routable);
            Assert.IsTrue(System.IO.File.Exists("MockOut/Archive/test.txt"));
        }

        [Test]
        [TestCase(LogLevel.Information)]
        [TestCase(LogLevel.Error)]
        public void CorrectLogRecieved(LogLevel logLevel)
        {
            LogLevel recieved = LogLevel.Debug;
            Mock<IWebRequestUtility> webRequestUtility = this.GetWebRequest();
            if (logLevel == LogLevel.Error)
            {
                webRequestUtility.Setup(x => x.UploadFile(It.IsAny<byte[]>(), It.IsAny<FtpSettings>(), It.IsAny<string>())).Throws(new OutOfMemoryException());
            }

            To to = new To(this.GetRoute().Headers, webRequestUtility.Object);
            Routable routable = new Routable(new Dictionary<string, string>()
            {
                { "SourceFile", "Test.txt" }
            },
            Encoding.UTF8.GetBytes("Hello")
            );
            to.OnLog += (sender, e) =>
            {
                recieved = e.LogLevel;
            };

            to.Process(routable);
            Assert.AreEqual(logLevel, recieved);
        }

        private void To_OnLog(object sender, Log e)
        {
            throw new NotImplementedException();
        }

        private Routable WriteFile()
        {
            this.PrepareArchiveTest();

            System.IO.File.WriteAllText("MockOut/Out/test.txt", "Hello");
            return new Routable(new Dictionary<string, string>()
            {
                { "FullSource", "MockOut/Out/test.txt" },
                {"SourceFile", "test.txt" }
            },
            Encoding.UTF8.GetBytes("Hello")
            );
        }

        private void PrepareArchiveTest()
        {
            if (Directory.Exists("MockOut/Out"))
            {
                Directory.Delete("MockOut", true);
            }

            Directory.CreateDirectory("MockOut/Out");
        }

        private Mock<IWebRequestUtility> GetWebRequest()
        {
            Mock<IWebRequestUtility> response = new Mock<IWebRequestUtility>();
            response.Setup(x => x.UploadFile(It.IsAny<byte[]>(), It.IsAny<FtpSettings>(), It.IsAny<string>())).Verifiable();
            return response;
        }

        private RouteAttributes GetRoute(bool archive = false, string source = "Body")
        {
            string archivePath = archive ? "&Archive=../Archive/" : string.Empty;
            return new RouteAttributes($"ftp://test@127.0.0.1/out{archivePath}&Source={source}");
        }
    }
}