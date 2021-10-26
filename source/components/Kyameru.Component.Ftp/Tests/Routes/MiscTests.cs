using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Kyameru.Component.Ftp.Extensions;

namespace Kyameru.Component.Ftp.Tests.Routes
{
    [TestFixture]
    public class MiscTests
    {
        [Test]
        public void StringisNullReurnsTrue()
        {
            Assert.True(string.Empty.IsNullOrEmptyPath());
        }

        [Test]
        [TestCase("path/", "path")]
        [TestCase("path", "path")]
        public void StringStripsPath(string input, string expected)
        {
            Assert.AreEqual(expected, input.StripEndingSlash());
        }
    }
}
