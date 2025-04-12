using Xunit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Kyameru.Component.File.Tests;

public class FileToTests
{
    private readonly string fileLocation;
    private IServiceProvider serviceProvider;
    private ServiceHelper serviceHelper = new ServiceHelper();

    public FileToTests()
    {
        fileLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace("\\", "/") + "/test";
        serviceProvider = serviceHelper.GetServiceProvider();
    }

    [Theory]
    [InlineData("Move", "String")]
    [InlineData("Copy", "String")]
    [InlineData("Write", "String")]
    [InlineData("Write", "Byte")]
    public async Task CanDoAction(string action, string bodyType)
    {
        var randomFileName = $"{Guid.NewGuid().ToString("N")}.txt";
        var fileTo = Setup(action, randomFileName);

        var routableHeaders = new Dictionary<string, string>()
        {
            { "FullSource", $"test/{randomFileName}" },
            { "SourceFile", randomFileName }
        };
        var routable = new Core.Entities.Routable(routableHeaders, System.Text.Encoding.UTF8.GetBytes("test file"));
        routable.SetBody<Byte[]>(System.Text.Encoding.UTF8.GetBytes("Test"));
        if (bodyType == "String")
        {
            routable.SetBody<string>("Test");
        }

        await fileTo.ProcessAsync(routable, default);
        Assert.True(System.IO.File.Exists($"{fileLocation}/target/{randomFileName}"));
    }

    [Theory]
    [InlineData("Move", "String")]
    [InlineData("Copy", "String")]
    [InlineData("Write", "String")]
    [InlineData("Write", "Byte")]
    public async Task CanDoActionAsync(string action, string bodyType)
    {
        var randomFileName = $"{Guid.NewGuid().ToString("N")}.txt";
        var fileTo = Setup(action, randomFileName);

        var routableHeaders = new Dictionary<string, string>()
        {
            { "FullSource", $"test/{randomFileName}" },
            { "SourceFile", randomFileName }
        };
        var routable = new Core.Entities.Routable(routableHeaders, System.Text.Encoding.UTF8.GetBytes("test file"));
        routable.SetBody<Byte[]>(System.Text.Encoding.UTF8.GetBytes("Test"));
        if (bodyType == "String")
        {
            routable.SetBody<string>("Test");
        }

        await fileTo.ProcessAsync(routable, default);
        Assert.True(System.IO.File.Exists($"{fileLocation}/target/{randomFileName}"));
    }

    [Fact]
    public async Task CanDeleteFile()
    {
        var randomFileName = $"{Guid.NewGuid():N}.txt";
        var fileTo = Setup("Delete", randomFileName);
        var routableHeaders = new Dictionary<string, string>()
        {
            { "FullSource", $"test/{randomFileName}" },
            { "SourceFile", randomFileName }
        };
        await fileTo.ProcessAsync(new Core.Entities.Routable(routableHeaders, System.Text.Encoding.UTF8.GetBytes("test file")), default);
        Assert.False(System.IO.File.Exists($"test/{randomFileName}"));
    }

    [Fact]
    public async Task CanDeleteFileAsync()
    {
        var randomFileName = $"{Guid.NewGuid():N}.txt";
        var fileTo = Setup("Delete", randomFileName);
        var routableHeaders = new Dictionary<string, string>()
        {
            { "FullSource", $"test/{randomFileName}" },
            { "SourceFile", randomFileName }
        };
        await fileTo.ProcessAsync(new Core.Entities.Routable(routableHeaders, System.Text.Encoding.UTF8.GetBytes("test file")), default);
        Assert.False(System.IO.File.Exists($"test/{randomFileName}"));
    }

    private FileTo Setup(string action, string randomFileName)
    {
        if (!Directory.Exists(fileLocation))
        {
            Directory.CreateDirectory(fileLocation);
        }

        System.IO.File.WriteAllText($"{fileLocation}/{randomFileName}", "test file");
        var headers = new Dictionary<string, string>()
        {
            { "Target", $"{fileLocation}/target" },
            { "Action", action }
        };


        return (FileTo)new Inflator().CreateToComponent(headers, serviceProvider);
    }
}