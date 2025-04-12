using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Kyameru.Component.File.Utilities;
using Xunit;

namespace Kyameru.Component.File.Tests;

public class FileUtilsTests
{

    private readonly string fileLocation;
    private readonly FileUtils fileUtils;
    private readonly string toFile;

    public FileUtilsTests()
    {
        fileLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace("\\", "/") + "/fileUtils";
        fileUtils = new FileUtils();
        toFile = $"{fileLocation}/test.txt";
        Init();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task WriteBytesOverwrites(bool overwrite)
    {
        var data = System.Text.Encoding.UTF8.GetBytes("Data");
        WriteFile(overwrite);
        var exception = await Record.ExceptionAsync(() => fileUtils.WriteAllBytesAsync(toFile, data, overwrite, default));
        Assert.Null(exception);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task WriteAlltextOverwrites(bool overwrite)
    {
        var data = "Data";
        WriteFile(overwrite);
        var exception = await Record.ExceptionAsync(() => fileUtils.WriteAllTextAsync(toFile, data, overwrite, default));
        Assert.Null(exception);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task MoveFileOverrites(bool overwrite)
    {
        var destination = $"{fileLocation}/to/dest.txt";
        Directory.CreateDirectory($"{fileLocation}/to");
        if (overwrite)
        {
            System.IO.File.WriteAllText(destination, "destinationfile");
        }

        System.IO.File.WriteAllText(toFile, "data");
        await fileUtils.MoveAsync(toFile, destination, overwrite, default);
        Assert.Equal("data", System.IO.File.ReadAllText(destination));
        Assert.False(System.IO.File.Exists(toFile));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CopyFileOverwrites(bool overwrite)
    {
        var destination = $"{fileLocation}/to/dest.txt";
        Directory.CreateDirectory($"{fileLocation}/to");
        if (overwrite)
        {
            System.IO.File.WriteAllText(destination, "destinationfile");
        }
        System.IO.File.WriteAllText(toFile, "data");
        await fileUtils.CopyFileAsync(toFile, destination, overwrite, default);
        Assert.Equal("data", System.IO.File.ReadAllText(destination));
        Assert.True(System.IO.File.Exists(toFile));
    }

    private void Init()
    {
        if (Directory.Exists(fileLocation))
        {
            Directory.Delete(fileLocation, true);
        }

        Directory.CreateDirectory(fileLocation);
    }

    private void WriteFile(bool overwrite)
    {
        if (overwrite)
        {
            System.IO.File.WriteAllText(toFile, "Overwrite Me");
        }
    }
}