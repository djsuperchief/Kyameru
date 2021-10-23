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
    }
}
