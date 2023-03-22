using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Component.File.Tests
{
    public class ConstructionExceptionTests
    {
        private readonly IServiceProvider serviceProvider;
        private ServiceHelper serviceHelper = new ServiceHelper();

        public ConstructionExceptionTests()
        {
            this.serviceProvider = this.serviceHelper.GetServiceProvider();
        }

        [Fact]
        public void EmptyTargetThrowsError()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                { "Notifications", "Changed" },
            };
            Inflator inflator = new Inflator();

            Assert.Throws<ArgumentException>(() => _ = inflator.CreateFromComponent(headers, false, this.serviceProvider));
        }

        [Fact]
        public void EmptyNotificationsThrowsError()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                { "Target", "C:/test" },
            };
            Inflator inflator = new Inflator();

            Assert.Throws<ArgumentException>(() => _ = inflator.CreateFromComponent(headers, false, this.serviceProvider));
        }
    }
}