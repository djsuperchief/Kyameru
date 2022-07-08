using Kyameru.Component.File.Utilities;
using Xunit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Kyameru.Component.File.Tests
{
    public class FileToTests
    {
        private readonly string fileLocation;
        private IServiceProvider serviceProvider;
        private ServiceHelper serviceHelper = new ServiceHelper();

        public FileToTests()
        {
            this.fileLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace("\\", "/") + "/test";
            this.serviceProvider = serviceHelper.GetServiceProvider();
        }

        [Theory]
        [InlineData("Move", "String")]
        [InlineData("Copy", "String")]
        [InlineData("Write", "String")]
        [InlineData("Write", "Byte")]
        public void CanDoAction(string action, string bodyType)
        {
            string randomFileName = $"{Guid.NewGuid().ToString("N")}.txt";
            FileTo fileTo = this.Setup(action, randomFileName);

            Dictionary<string, string> routableHeaders = new Dictionary<string, string>()
            {
                { "FullSource", $"test/{randomFileName}" },
                { "SourceFile", randomFileName }
            };
            Core.Entities.Routable routable = new Core.Entities.Routable(routableHeaders, System.Text.Encoding.UTF8.GetBytes("test file"));
            routable.SetBody<Byte[]>(System.Text.Encoding.UTF8.GetBytes("Test"));
            if(bodyType == "String")
            {
                routable.SetBody<string>("Test");
            }

            fileTo.Process(routable);
            Assert.True(System.IO.File.Exists($"{this.fileLocation}/target/{randomFileName}"));
        }

        [Fact]
        public void CanDeleteFile()
        {
            string randomFileName = $"{Guid.NewGuid().ToString("N")}.txt";
            FileTo fileTo = this.Setup("Delete", randomFileName);
            Dictionary<string, string> routableHeaders = new Dictionary<string, string>()
            {
                { "FullSource", $"test/{randomFileName}" },
                { "SourceFile", randomFileName }
            };
            fileTo.Process(new Core.Entities.Routable(routableHeaders, System.Text.Encoding.UTF8.GetBytes("test file")));
            Assert.False(System.IO.File.Exists($"test/{randomFileName}"));
        }

        private FileTo Setup(string action, string randomFileName)
        {
            if (!Directory.Exists(fileLocation))
            {
                Directory.CreateDirectory(fileLocation);
            }

            System.IO.File.WriteAllText($"{fileLocation}/{randomFileName}", "test file");
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                { "Target", $"{this.fileLocation}/target" },
                { "Action", action }
            };
            

            return (FileTo)new Inflator().CreateToComponent(headers, this.serviceProvider);

            //return new FileTo(headers, new Component.File.Utilities.FileUtils());
        }
    }
}