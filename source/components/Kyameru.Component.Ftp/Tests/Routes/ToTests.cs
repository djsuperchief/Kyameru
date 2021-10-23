using Kyameru.Component.Ftp.Contracts;
using Kyameru.Component.Ftp.Settings;
using Kyameru.Core.Entities;
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
        public void CanUploadFile()
        {
            Mock<IWebRequestUtility> webRequestUtility = this.GetWebRequest();
            To to = new To(this.GetRoute().Headers, webRequestUtility.Object);
            Routable routable = new Routable(new Dictionary<string, string>()
            {
                { "SourceFile", "Test.txt" }
            },
            Encoding.UTF8.GetBytes("Hello")
            );
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