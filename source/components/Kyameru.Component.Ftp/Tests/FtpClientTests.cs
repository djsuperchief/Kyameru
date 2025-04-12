using Kyameru.Component.Ftp.Contracts;
using Kyameru.Component.Ftp.Settings;
using Kyameru.Core.Entities;
using Kyameru.TestUtilities;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Kyameru.Component.Ftp.Tests;

public class FtpClientTests
{
    //private readonly Mock<IWebRequestUtility> webRequestUtility = new Mock<IWebRequestUtility>();

    [Fact]
    public async Task UploadThrowsError()
    {
        var webRequestUtility = Substitute.For<IWebRequestUtility>();
        webRequestUtility.UploadFile(default, default, default, default).Throws(new OutOfMemoryException());
        CreateTestFile();
        var client = new FtpClient(this.GetFtpSettings(), webRequestUtility);
        bool errorThrown = false;
        client.OnError += (sender, e) =>
        {
            errorThrown = true;
        };

        await Assert.ThrowsAsync<OutOfMemoryException>(() => client.UploadFile("bloop.txt", default));
        Assert.True(errorThrown);

    }

    [Fact]
    public async Task GetDirectoryContentsErrors()
    {
        var webRequestUtility = Substitute.For<IWebRequestUtility>();
        webRequestUtility.GetDirectoryContents(default, default).Throws(new OutOfMemoryException());

        RouteAttributes route = new RouteAttributes($"ftp://test:banana@127.0.0.1/out&Delete=true&PollTime=1");
        From from = new From(route.Headers, webRequestUtility);
        var thread = TestThread.CreateNew(from.StartAsync, 5);
        bool errorThrown = false;
        from.OnLog += (sender, e) =>
        {
            if (e.LogLevel == Microsoft.Extensions.Logging.LogLevel.Error)
            {
                errorThrown = true;

            }
        };

        from.Setup();
        thread.StartAndWait();
        Assert.True(errorThrown);
    }

    [Fact]
    public async Task DownloadFileErrors()
    {
        AutoResetEvent resetEvent = new AutoResetEvent(false);
        this.webRequestUtility.Reset();
        this.webRequestUtility.Setup(x => x.GetDirectoryContents(It.IsAny<FtpSettings>(), It.IsAny<CancellationToken>())).Returns((FtpSettings f, CancellationToken c) =>
        {
            return Task.FromResult(new List<string>() { "file.txt" });

        });
        this.webRequestUtility.Setup(x => x.DownloadFile(It.IsAny<string>(), It.IsAny<FtpSettings>(), It.IsAny<CancellationToken>())).Throws(new OutOfMemoryException());
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
        await from.StartAsync(default);
        bool wasAssigned = resetEvent.WaitOne(TimeSpan.FromSeconds(5));
        Assert.True(errorThrown);

    }

    [Fact]
    public async Task DeleteFileErrors()
    {
        AutoResetEvent resetEvent = new AutoResetEvent(false);
        this.webRequestUtility.Reset();
        this.webRequestUtility.Setup(x => x.GetDirectoryContents(It.IsAny<FtpSettings>(), It.IsAny<CancellationToken>())).Returns((FtpSettings f, CancellationToken c) =>
        {
            return Task.FromResult(new List<string>() { "file.txt" });
        });
        this.webRequestUtility.Setup(x => x.DeleteFile(It.IsAny<FtpSettings>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).Throws(new OutOfMemoryException());
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
        await from.StartAsync(default);
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
