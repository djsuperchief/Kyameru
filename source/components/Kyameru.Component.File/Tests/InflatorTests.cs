using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Component.File.Tests
{
    [TestFixture]
    public class InflatorTests
    {
        private IServiceProvider serviceProvider;
        private ServiceHelper serviceHelper = new ServiceHelper();

        [SetUp]
        public void Init()
        {
            this.serviceProvider = this.serviceHelper.GetServiceProvider();
        }

        [Test]
        public void CanGetFrom()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                { "Target", "test/" },
                { "Notifications", "Created" },
                { "Filter", "*.tdd" },
                { "SubDirectories", "true" }
            };
            Inflator inflator = new Inflator();
            Assert.NotNull(inflator.CreateFromComponent(headers, false, this.serviceProvider));
        }

        [Test]
        public void CanGetTo()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                { "Target", "test/target" },
                { "Action", "Move" },
                { "Overwrite","true" }
            };

            Inflator inflator = new Inflator();
            Assert.NotNull(inflator.CreateToComponent(headers, this.serviceProvider));
        }

        [Test]
        public void AtomicThrows()
        {
            Inflator inflator = new Inflator();
            Assert.Throws<NotImplementedException>(() => inflator.CreateAtomicComponent(null));
        }
    }
}