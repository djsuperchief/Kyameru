using Kyameru.Component.Ftp.Contracts;
using Kyameru.Component.Ftp.Settings;
using Kyameru.Core.Entities;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Kyameru.Component.Ftp.Tests
{
    [TestFixture]
    public class FtpClientTests
    {
        private readonly Mock<IWebRequestUtility> webRequestUtility = new Mock<IWebRequestUtility>();

        [Test]
        public void UploadThrowsError()
        {
            this.webRequestUtility.Reset();
            this.webRequestUtility.Setup(x => x.UploadFile(It.IsAny<byte[]>(), It.IsAny<FtpSettings>(), It.IsAny<string>())).Throws(new OutOfMemoryException());
            this.CreateTestFile();
            FtpClient client = new FtpClient(this.GetFtpSettings(), this.webRequestUtility.Object);
            bool errorThrown = false;
            client.OnError += (sender, e) =>
            {
                errorThrown = true;
            };

            Assert.Throws<OutOfMemoryException>(() => client.UploadFile("bloop.txt"));
            Assert.True(errorThrown);

        }

        [Test]
        public void GetDirectoryContentsErrors()
        {
            AutoResetEvent resetEvent = new AutoResetEvent(false);
            this.webRequestUtility.Reset();
            this.webRequestUtility.Setup(x => x.GetDirectoryContents(It.IsAny<FtpSettings>())).Throws(new OutOfMemoryException());
            RouteAttributes route = new RouteAttributes($"ftp://test:banana@127.0.0.1/out&Delete=true&PollTime=1");
            From from = new From(route.Headers, webRequestUtility.Object);
            bool errorThrown = false;
            from.OnLog += (sender, e) =>
            {
                if (e.LogLevel == Microsoft.Extensions.Logging.LogLevel.Error)
                {
                    errorThrown = true;
                    resetEvent.Set();
                }
            };

            from.Setup();
            from.Start();
            bool wasAssigned = resetEvent.WaitOne(TimeSpan.FromSeconds(5));
            Assert.True(errorThrown);

        }

        [Test]
        public void DownloadFileErrors()
        {
            AutoResetEvent resetEvent = new AutoResetEvent(false);
            this.webRequestUtility.Reset();
            this.webRequestUtility.Setup(x => x.GetDirectoryContents(It.IsAny<FtpSettings>())).Returns(new List<string>() { "file.txt" });
            this.webRequestUtility.Setup(x => x.DownloadFile(It.IsAny<string>(), It.IsAny<FtpSettings>())).Throws(new OutOfMemoryException());
            RouteAttributes route = new RouteAttributes($"ftp://test:banana@127.0.0.1/out&Delete=true&PollTime=1");
            From from = new From(route.Headers, webRequestUtility.Object);
            bool errorThrown = false;
            from.OnLog += (sender, e) =>
            {
                if (e.LogLevel == Microsoft.Extensions.Logging.LogLevel.Error)
                {
                    errorThrown = true;
                    resetEvent.Set();
                }
            };

            from.Setup();
            from.Start();
            bool wasAssigned = resetEvent.WaitOne(TimeSpan.FromSeconds(5));
            Assert.True(errorThrown);

        }

        [Test]
        public void DeleteFileErrors()
        {
            AutoResetEvent resetEvent = new AutoResetEvent(false);
            this.webRequestUtility.Reset();
            this.webRequestUtility.Setup(x => x.GetDirectoryContents(It.IsAny<FtpSettings>())).Returns(new List<string>() { "file.txt" });
            this.webRequestUtility.Setup(x => x.DeleteFile(It.IsAny<FtpSettings>(),It.IsAny<string>(),It.IsAny<bool>())).Throws(new OutOfMemoryException());
            RouteAttributes route = new RouteAttributes($"ftp://test:banana@127.0.0.1/out&Delete=true&PollTime=1");
            From from = new From(route.Headers, webRequestUtility.Object);
            bool errorThrown = false;
            from.OnLog += (sender, e) =>
            {
                if (e.LogLevel == Microsoft.Extensions.Logging.LogLevel.Error)
                {
                    errorThrown = true;
                    resetEvent.Set();
                }
            };

            from.Setup();
            from.Start();
            bool wasAssigned = resetEvent.WaitOne(TimeSpan.FromSeconds(5));
            Assert.True(errorThrown);

        }

        private void CreateTestFile()
        {
            using (StreamWriter sw = File.CreateText("bloop.txt"))
            {
                sw.WriteLine("Hello");
                sw.WriteLine("And");
                sw.WriteLine("Welcome");
            }
        }

        private FtpSettings GetFtpSettings()
        {
            RouteAttributes route = new RouteAttributes($"ftp://test:banana@127.0.0.1/out&Delete=true&PollTime=5000");
            return new FtpSettings(ConfigSetup.ToFromConfig(route.Headers));
        }
    }
}
