using Kyameru.Component.Ftp.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Kyameru.Component.Ftp.Tests.Routes;

public class ToTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CanUploadFile(bool stringBody)
    {
        IWebRequestUtility webRequestUtility = this.GetWebRequest();
        To to = new To(this.GetRoute().Headers, webRequestUtility);
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
        await webRequestUtility.ReceivedWithAnyArgs().UploadFile(default, default, default, default);
    }

    [Fact]
    public async Task CanUploadFileAsync()
    {
        var tokenSource = new CancellationTokenSource();
        var webRequestUtility = this.GetWebRequest();
        To to = new To(this.GetRoute().Headers, webRequestUtility);
        Routable routable = new Routable(new Dictionary<string, string>()
            {
                { "SourceFile", "Test.txt" }
            },
            "Hello"u8.ToArray()
        );

        await to.ProcessAsync(routable, tokenSource.Token);
        await webRequestUtility.ReceivedWithAnyArgs().UploadFile(default, default, default, default);
    }

    [Fact]
    public async Task CanUploadAndArchive()
    {
        IWebRequestUtility webRequestUtility = this.GetWebRequest();
        To to = new To(this.GetRoute(true, "File").Headers, webRequestUtility);
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
        var webRequestUtility = Substitute.For<IWebRequestUtility>();
        if (logLevel == LogLevel.Error)
        {
            webRequestUtility.UploadFile(default, default, default, default).ThrowsAsyncForAnyArgs<OutOfMemoryException>();
        }

        To to = new To(this.GetRoute().Headers, webRequestUtility);
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

    private IWebRequestUtility GetWebRequest()
    {
        return Substitute.For<IWebRequestUtility>();
    }

    private RouteAttributes GetRoute(bool archive = false, string source = "Body")
    {
        string archivePath = archive ? "&Archive=../Archive/" : string.Empty;
        return new RouteAttributes($"ftp://test@127.0.0.1/out{archivePath}&Source={source}");
    }
}