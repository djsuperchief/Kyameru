using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Component.File.Utilities;
using Kyameru.Core.Sys;
using NSubstitute;
using Xunit;

namespace Kyameru.Component.File.Tests;

public class FileWatcherTests
{
    private readonly string location;
    private readonly IFileSystemWatcher fileSystemWatcher = Substitute.For<IFileSystemWatcher>();

    public FileWatcherTests()
    {
        location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/test";
    }

    [Fact]
    public async Task CreatedWorks()
    {
        var tokenSource = new CancellationTokenSource();
        CheckFile("created.tdd");
        var resetEvent = new AutoResetEvent(false);
        var method = string.Empty;
        var raisedAsync = false;
        // Github tests for some reason do not raise created compared to local os.
        FileWatcher from = Setup("Created");

        from.OnActionAsync += async delegate (object sender, RoutableEventData e)
        {
            method = e.Data.Headers["Method"];
            raisedAsync = true;
            resetEvent.Set();
            await Task.CompletedTask;
        };

        from.Setup();
        await from.StartAsync(tokenSource.Token);
        System.IO.File.WriteAllText($"{location}/Created.tdd", "test data");
        fileSystemWatcher.Created += Raise.Event<FileSystemEventHandler>(new FileSystemEventArgs(WatcherChangeTypes.Created, location, "Created.tdd"));
        bool wasAssigned = resetEvent.WaitOne(TimeSpan.FromSeconds(5));

        await from.StopAsync(tokenSource.Token);

        Assert.True(!string.IsNullOrWhiteSpace(method));
    }

    [Fact]
    public async Task ChangedWorks()
    {
        var tokenSource = new CancellationTokenSource();
        string filename = $"{Guid.NewGuid().ToString("N")}.txt";
        CheckFile(filename);
        var resetEvent = new AutoResetEvent(false);
        System.IO.File.WriteAllText($"{location}/{filename}", "test data");
        string method = string.Empty;

        FileWatcher from = Setup("Changed");

        from.OnActionAsync += async delegate (object sender, RoutableEventData e)
        {
            method = e.Data.Headers["Method"];
            //resetEvent.Set();
            await Task.CompletedTask;
        };
        from.Setup();

        await from.StartAsync(tokenSource.Token);

        System.IO.File.WriteAllText($"{location}/{filename}", "more data added");
        System.IO.File.WriteAllText($"{location}/{filename}", "more data added");
        fileSystemWatcher.Changed += Raise.Event<FileSystemEventHandler>(new FileSystemEventArgs(WatcherChangeTypes.Changed, location, filename));
        bool wasAssigned = resetEvent.WaitOne(TimeSpan.FromSeconds(5));

        // doing this async means it can be scanned before changed...also async tests, need a better way of
        // testing this.
        Assert.Equal("Changed", method);
    }

    [Theory]
    [InlineData("true", 21)]
    [InlineData("false", 1)]
    public async Task ScannerWorks(string subDirectories, int expected)
    {
        string contents = "test data";
        string scanDir = location + "scan";
        int count = 0;
        AutoResetEvent resetEvent = new AutoResetEvent(false);

        if (System.IO.Directory.Exists(scanDir))
        {
            Directory.Delete(scanDir, true);
        }
        Directory.CreateDirectory(scanDir);
        Directory.CreateDirectory(Path.Combine(scanDir, "sub"));

        for (int i = 0; i < 20; i++)
        {
            System.IO.File.WriteAllText($"{scanDir}/sub/testfile{i}.txt", contents);
        }

        System.IO.File.WriteAllText($"{scanDir}/testfile_root.txt", contents);

        FileWatcher from = Setup("Changed", true, scanDir, "", "", subDirectories);
        from.OnActionAsync += async (object sender, RoutableEventData e) =>
        {
            if (e.Data.Headers["Method"] == "Scanned")
            {
                count++;
            }

            await Task.CompletedTask;

        };
        from.Setup();
        await from.StartAsync(default);
        resetEvent.WaitOne(TimeSpan.FromSeconds(5));
        Assert.Equal(expected, count);
    }

    [Theory]
    [InlineData("in|inner", "")]
    [InlineData("", ".stuff|stuffing")]
    public async Task IgnoreWorks(string directories, string strings)
    {
        string contents = "test data";
        string scanDir = "in";
        string fileName = "myfile.txt";
        if (!string.IsNullOrWhiteSpace(strings))
        {
            fileName = $"{fileName}{strings.SplitPiped()[0]}";
        }

        int count = 0;
        AutoResetEvent resetEvent = new AutoResetEvent(false);

        if (System.IO.Directory.Exists(scanDir))
        {
            Directory.Delete(scanDir, true);
        }
        Directory.CreateDirectory(scanDir);
        System.IO.File.WriteAllText(Path.Combine(scanDir, fileName), contents);

        FileWatcher from = Setup("Changed", true, scanDir, directories, strings);
        from.OnActionAsync += async (object sender, RoutableEventData e) =>
        {
            if (e.Data.Headers["Method"] == "Scanned")
            {
                count++;
            }

            await Task.CompletedTask;
        };
        from.Setup();
        await from.StartAsync(default);
        resetEvent.WaitOne(TimeSpan.FromSeconds(5));
        Assert.Equal(0, count);
    }

    public File.FileWatcher Setup(
        string notification,
        bool initialScan = false,
        string target = "test/",
        string ignore = "",
        string ignoreStrings = "",
        string subDirectories = "true")
    {
        Dictionary<string, string> headers = new Dictionary<string, string>()
        {
            { "Target",target },
            { "Notifications", notification },
            { "SubDirectories", subDirectories },
            { "InitialScan", initialScan.ToString() },
            { "Ignore", ignore },
            { "IgnoreStrings", ignoreStrings }
        };

        return new FileWatcher(headers, fileSystemWatcher);
    }

    private void CheckFile(string file)
    {
        if (!Directory.Exists(location))
        {
            Directory.CreateDirectory(location);
        }

        if (System.IO.File.Exists($"{location}/{file}"))
        {
            System.IO.File.Delete($"{location}/{file}");
        }
    }
}