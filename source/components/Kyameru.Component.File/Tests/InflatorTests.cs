using System;
using System.Collections.Generic;
using System.Text;
using Kyameru.Core.Exceptions;
using Xunit;

namespace Kyameru.Component.File.Tests
{
    public class InflatorTests
    {
        private IServiceProvider serviceProvider;
        private ServiceHelper serviceHelper = new ServiceHelper();

        public InflatorTests()
        {
            this.serviceProvider = this.serviceHelper.GetServiceProvider();
        }

        [Fact]
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

        [Fact]
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

        [Fact]
        public void AtomicThrows()
        {
            Inflator inflator = new Inflator();
            Assert.Throws<RouteNotAvailableException>(() => inflator.CreateAtomicComponent(null));
        }
    }
}