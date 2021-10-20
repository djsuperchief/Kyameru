using Kyameru.Component.File.Utilities;
using Kyameru.Core.Entities;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Component.File.Tests
{
    [TestFixture]
    public class FileToExceptionTests
    {
        private readonly Mock<IFileUtils> fileUtils = new Mock<IFileUtils>();

        [Test]
        [TestCase("Move")]
        [TestCase("Copy")]
        [TestCase("Delete")]
        [TestCase("Write")]
        public void ActionSetsError(string action)
        {
            FileTo fileTo = this.GetFileTo(action);
            Routable message = this.GetRoutable();
            fileTo.Process(message);
            Assert.NotNull(message.Error);
        }

        [SetUp]
        public void Init()
        {
            this.fileUtils.Setup(x => x.CopyFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Throws(new NotImplementedException());
            this.fileUtils.Setup(x => x.CreateDirectory(It.IsAny<string>())).Throws(new NotImplementedException());
            this.fileUtils.Setup(x => x.Delete(It.IsAny<string>())).Throws(new NotImplementedException());
            this.fileUtils.Setup(x => x.Move(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Throws(new NotImplementedException());
            this.fileUtils.Setup(x => x.WriteAllBytes(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<bool>())).Throws(new NotImplementedException());
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
            }, this.fileUtils.Object);
        }
    }
}