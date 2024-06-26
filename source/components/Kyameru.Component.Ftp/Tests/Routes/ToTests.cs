﻿using Kyameru.Component.Ftp.Contracts;
using Kyameru.Component.Ftp.Settings;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Kyameru.Component.Ftp.Tests.Routes
{
    public class ToTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task CanUploadFile(bool stringBody)
        {
            Mock<IWebRequestUtility> webRequestUtility = this.GetWebRequest();
            To to = new To(this.GetRoute().Headers, webRequestUtility.Object);
            Routable routable = new Routable(new Dictionary<string, string>()
            {
                { "SourceFile", "Test.txt" }
            },
            Encoding.UTF8.GetBytes("Hello")
            );
            if (stringBody)
            {
                routable.SetBody<string>("Hello");
            }

            await to.ProcessAsync(routable, default);
            webRequestUtility.VerifyAll();
        }

        [Fact]
        public async Task CanUploadFileAsync()
        {
            var tokenSource = new CancellationTokenSource();
            Mock<IWebRequestUtility> webRequestUtility = this.GetWebRequest();
            To to = new To(this.GetRoute().Headers, webRequestUtility.Object);
            Routable routable = new Routable(new Dictionary<string, string>()
                {
                    { "SourceFile", "Test.txt" }
                },
                "Hello"u8.ToArray()
            );

            await to.ProcessAsync(routable, tokenSource.Token);
            webRequestUtility.VerifyAll();
        }

        [Fact]
        public async Task CanUploadAndArchive()
        {
            Mock<IWebRequestUtility> webRequestUtility = this.GetWebRequest();
            To to = new To(this.GetRoute(true, "File").Headers, webRequestUtility.Object);
            Routable routable = this.WriteFile();
            await to.ProcessAsync(routable, default);
            Assert.True(System.IO.File.Exists("MockOut/Archive/test.txt"));
        }

        [Theory]
        [InlineData(LogLevel.Information)]
        [InlineData(LogLevel.Error)]
        public async Task CorrectLogReceived(LogLevel logLevel)
        {
            LogLevel received = LogLevel.Debug;
            Mock<IWebRequestUtility> webRequestUtility = this.GetWebRequest();
            if (logLevel == LogLevel.Error)
            {
                webRequestUtility.Setup(x => x.UploadFile(It.IsAny<byte[]>(), It.IsAny<FtpSettings>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Throws(new OutOfMemoryException());
            }

            To to = new To(this.GetRoute().Headers, webRequestUtility.Object);
            Routable routable = new Routable(new Dictionary<string, string>()
            {
                { "SourceFile", "Test.txt" },
                {"FileName", "Test.txt" }
            },
            Encoding.UTF8.GetBytes("Hello")
            );
            to.OnLog += (sender, e) =>
            {
                received = e.LogLevel;
            };

            await to.ProcessAsync(routable, default);
            Assert.Equal(logLevel, received);
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
            response.Setup(x => x.UploadFile(It.IsAny<byte[]>(), It.IsAny<FtpSettings>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Verifiable();
            return response;
        }

        private RouteAttributes GetRoute(bool archive = false, string source = "Body")
        {
            string archivePath = archive ? "&Archive=../Archive/" : string.Empty;
            return new RouteAttributes($"ftp://test@127.0.0.1/out{archivePath}&Source={source}");
        }
    }
}