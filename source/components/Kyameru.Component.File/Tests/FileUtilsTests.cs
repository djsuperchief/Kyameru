using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Kyameru.Component.File.Utilities;
using Xunit;

namespace Kyameru.Component.File.Tests
{
    public class FileUtilsTests
    {

        private readonly string fileLocation;
        private readonly FileUtils fileUtils;
        private readonly string toFile;

        public FileUtilsTests()
        {
            this.fileLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace("\\", "/") + "/fileUtils";
            this.fileUtils = new FileUtils();
            this.toFile = $"{this.fileLocation}/test.txt";
            this.Init();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task WriteBytesOverwrites(bool overwrite)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes("Data");
            this.WriteFile(overwrite);
            var exception = await Record.ExceptionAsync(() => this.fileUtils.WriteAllBytesAsync(toFile, data, overwrite, default));
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task WriteAlltextOverwrites(bool overwrite)
        {
            string data = "Data";
            this.WriteFile(overwrite);
            var exception = await Record.ExceptionAsync(() => this.fileUtils.WriteAllTextAsync(this.toFile, data, overwrite, default));
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task MoveFileOverrites(bool overwrite)
        {
            string destination = $"{this.fileLocation}/to/dest.txt";
            Directory.CreateDirectory($"{this.fileLocation}/to");
            if (overwrite)
            {
                System.IO.File.WriteAllText(destination, "destinationfile");
            }

            System.IO.File.WriteAllText(toFile, "data");
            await this.fileUtils.MoveAsync(toFile, destination, overwrite, default);
            Assert.Equal("data", System.IO.File.ReadAllText(destination));
            Assert.False(System.IO.File.Exists(this.toFile));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task CopyFileOverwrites(bool overwrite)
        {
            string destination = $"{this.fileLocation}/to/dest.txt";
            Directory.CreateDirectory($"{this.fileLocation}/to");
            if (overwrite)
            {
                System.IO.File.WriteAllText(destination, "destinationfile");
            }
            System.IO.File.WriteAllText(toFile, "data");
            await this.fileUtils.CopyFileAsync(toFile, destination, overwrite, default);
            Assert.Equal("data", System.IO.File.ReadAllText(destination));
            Assert.True(System.IO.File.Exists(this.toFile));
        }

        private void Init()
        {
            if (Directory.Exists(this.fileLocation))
            {
                Directory.Delete(this.fileLocation, true);
            }

            Directory.CreateDirectory(this.fileLocation);
        }

        private void WriteFile(bool overwrite)
        {
            if (overwrite)
            {
                System.IO.File.WriteAllText(toFile, "Overwrite Me");
            }
        }
    }
}