using Kyameru.Core.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Component.File.Utilities;
using Kyameru.Core;
using Xunit;

namespace Kyameru.Component.File.Tests
{
    public class FileWatcherTests
    {
        private readonly string location;
        private readonly Mock<Component.File.Utilities.IFileSystemWatcher> fileSystemWatcher = new Mock<Utilities.IFileSystemWatcher>();

        public FileWatcherTests()
        {
            this.location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/test";
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task CreatedWorks(bool isAsync)
        {
            var tokenSource = new CancellationTokenSource();
            this.CheckFile("created.tdd");
            AutoResetEvent resetEvent = new AutoResetEvent(false);
            var method = string.Empty;
            var raisedAsync = false;
            // Github tests for some reason do not raise created compared to local os.
            FileWatcher from = this.Setup("Created");
            from.OnAction += delegate (object sender, Routable e)
            {
                method = e.Headers["Method"];
                
                resetEvent.Set();
            };

            from.OnActionAsync += async delegate(object sender, RoutableEventData e)
            {
                method = e.Data.Headers["Method"];
                raisedAsync = true;
                resetEvent.Set();
                await Task.CompletedTask;
            };
            
            from.Setup();
            if (isAsync)
            {
                
                await from.StartAsync(tokenSource.Token);
            }
            else
            {
                from.Start();
            }

            System.IO.File.WriteAllText($"{this.location}/Created.tdd", "test data");
            this.fileSystemWatcher.Raise(x => x.Created += null, new FileSystemEventArgs(WatcherChangeTypes.Created, this.location, "Created.tdd"));
            bool wasAssigned = resetEvent.WaitOne(TimeSpan.FromSeconds(5));

            if (isAsync)
            {
                await from.StopAsync(tokenSource.Token);
            }
            else
            {
                from.Stop();    
            }
            
            Assert.True(!string.IsNullOrWhiteSpace(method));
            Assert.Equal(isAsync, raisedAsync);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ChangedWorks(bool isAsync)
        {
            var tokenSource = new CancellationTokenSource();
            var raisedAsync = false;
            string filename = $"{Guid.NewGuid().ToString("N")}.txt";
            this.CheckFile(filename);
            AutoResetEvent resetEvent = new AutoResetEvent(false);
            System.IO.File.WriteAllText($"{this.location}/{filename}", "test data");
            string method = string.Empty;

            FileWatcher from = this.Setup("Changed");
            from.OnAction += delegate (object sender, Routable e)
            {
                method = e.Headers["Method"];
                resetEvent.Set();
            };

            from.OnActionAsync += async delegate(object sender, RoutableEventData e)
            {
                method = e.Data.Headers["Method"];
                raisedAsync = true;
                resetEvent.Set();
                await Task.CompletedTask;
            };
            from.Setup();
            if (isAsync)
            {
                await from.StartAsync(tokenSource.Token);
            }
            else
            {
                from.Start();
            }

            System.IO.File.WriteAllText($"{this.location}/{filename}", "more data added");
            System.IO.File.WriteAllText($"{this.location}/{filename}", "more data added");
            this.fileSystemWatcher.Raise(x => x.Changed += null, new FileSystemEventArgs(WatcherChangeTypes.Changed, this.location, filename));
            bool wasAssigned = resetEvent.WaitOne(TimeSpan.FromSeconds(5));
            if (!isAsync)
            {
                // doing this async means it can be scanned before changed...also async tests, need a better way of
                // testing this.
                Assert.Equal("Changed", method);    
            }
            
            Assert.Equal(isAsync, raisedAsync);
        }

        [Theory]
        [InlineData("true", 21)]
        [InlineData("false", 1)]
        public void ScannerWorks(string subDirectories, int expected)
        {
            string contents = "test data";
            string scanDir = this.location + "scan";
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

            FileWatcher from = this.Setup("Changed", true, scanDir, "", "", subDirectories);
            from.OnAction += delegate (object sender, Routable e)
            {
                if (e.Headers["Method"] == "Scanned")
                {
                    count++;
                }
            };
            from.Setup();
            from.Start();
            resetEvent.WaitOne(TimeSpan.FromSeconds(5));
            Assert.Equal(expected, count);
        }

        [Theory]
        [InlineData("in|inner", "")]
        [InlineData("", ".stuff|stuffing")]
        public void IgnoreWorks(string directories, string strings)
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

            FileWatcher from = this.Setup("Changed", true, scanDir, directories, strings);
            from.OnAction += delegate (object sender, Routable e)
            {
                if (e.Headers["Method"] == "Scanned")
                {
                    count++;
                }
            };
            from.Setup();
            from.Start();
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

            return new FileWatcher(headers, this.fileSystemWatcher.Object);
        }

        private void CheckFile(string file)
        {
            if (!Directory.Exists(this.location))
            {
                Directory.CreateDirectory(this.location);
            }

            if (System.IO.File.Exists($"{location}/{file}"))
            {
                System.IO.File.Delete($"{location}/{file}");
            }
        }
    }
}