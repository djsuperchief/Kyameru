using System;
using System.IO;
using System.Reflection;
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
        public void WriteBytesOverwrites(bool overwrite)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes("Data");
            this.WriteFile(overwrite);
            var exception = Record.Exception(() => this.fileUtils.WriteAllBytes(toFile, data, overwrite));
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void WriteAlltextOverwrites(bool overwrite)
        {
            string data = "Data";
            this.WriteFile(overwrite);
            var exception = Record.Exception(() => this.fileUtils.WriteAllText(this.toFile, data, overwrite));
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MoveFileOverrites(bool overwrite)
        {
            string destination = $"{this.fileLocation}/to/dest.txt";
            Directory.CreateDirectory($"{this.fileLocation}/to");
            if (overwrite)
            {
                System.IO.File.WriteAllText(destination, "destinationfile");
            }

            System.IO.File.WriteAllText(toFile, "data");
            this.fileUtils.Move(toFile, destination, overwrite);
            Assert.Equal("data", System.IO.File.ReadAllText(destination));
            Assert.False(System.IO.File.Exists(this.toFile));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CopyFileOverwrites(bool overwrite)
        {
            string destination = $"{this.fileLocation}/to/dest.txt";
            Directory.CreateDirectory($"{this.fileLocation}/to");
            if(overwrite)
            {
                System.IO.File.WriteAllText(destination, "destinationfile");
            }
            System.IO.File.WriteAllText(toFile, "data");
            this.fileUtils.CopyFile(toFile, destination, overwrite);
            Assert.Equal("data", System.IO.File.ReadAllText(destination));
            Assert.True(System.IO.File.Exists(this.toFile));
        }

        private void Init()
        {
            if(Directory.Exists(this.fileLocation))
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