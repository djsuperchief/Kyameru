﻿using Kyameru.Component.File.Utilities;
using Kyameru.Core.Entities;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Kyameru.Component.File.Tests
{
    public class FileToExceptionTests
    {
        private readonly Mock<IFileUtils> fileUtils = new Mock<IFileUtils>();

        [Theory]
        [InlineData("Move")]
        [InlineData("Copy")]
        [InlineData("Delete")]
        [InlineData("Write")]
        public async Task ActionSetsError(string action)
        {
            this.Init();
            FileTo fileTo = this.GetFileTo(action);
            Routable message = this.GetRoutable();
            await fileTo.ProcessAsync(message, default);
            Assert.NotNull(message.Error);
        }

        private void Init()
        {
            this.fileUtils.Reset();
            this.fileUtils.Setup(x => x.CopyFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).Throws(new NotImplementedException());
            this.fileUtils.Setup(x => x.CreateDirectoryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Throws(new NotImplementedException());
            this.fileUtils.Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Throws(new NotImplementedException());
            this.fileUtils.Setup(x => x.MoveAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).Throws(new NotImplementedException());
            this.fileUtils.Setup(x => x.WriteAllBytesAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).Throws(new NotImplementedException());
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