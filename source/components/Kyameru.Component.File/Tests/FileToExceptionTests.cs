using Kyameru.Component.File.Utilities;
using Kyameru.Core.Entities;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Kyameru.Component.File.Tests;

public class FileToExceptionTests
{
    private readonly IFileUtils fileUtils = Substitute.For<IFileUtils>();

    [Theory]
    [InlineData("Move")]
    [InlineData("Copy")]
    [InlineData("Delete")]
    [InlineData("Write")]
    public async Task ActionSetsError(string action)
    {
        this.Init();
        var fileTo = this.GetFileTo(action);
        var message = this.GetRoutable();
        await fileTo.ProcessAsync(message, default);
        Assert.NotNull(message.Error);
    }

    private void Init()
    {
        fileUtils.ClearReceivedCalls();
        fileUtils.CopyFileAsync(default, default, default, default).ThrowsAsyncForAnyArgs(new NotImplementedException());
        fileUtils.DeleteAsync(default, default).ThrowsAsyncForAnyArgs(new NotImplementedException());
        fileUtils.CreateDirectoryAsync(default, default).ThrowsAsyncForAnyArgs(new NotImplementedException());
        fileUtils.MoveAsync(default, default, default, default).ThrowsAsyncForAnyArgs(new NotImplementedException());
        fileUtils.WriteAllBytesAsync(default, default, default, default).ThrowsAsyncForAnyArgs(new NotImplementedException());
    }

    private Routable GetRoutable()
    {
        return new Routable(new Dictionary<string, string>()
        {
            { "FullSource", "test/test.txt" },
            { "SourceFile", "test.txt" }
        },
        System.Text.Encoding.UTF8.GetBytes("test file"));
    }

    private FileTo GetFileTo(string action)
    {
        return new FileTo(new Dictionary<string, string>()
        {
            { "Target", $"test/target" },
            { "Action", action }
        }, fileUtils);
    }
}