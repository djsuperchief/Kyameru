using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Kyameru.Component.File.Tests
{
    [TestFixture]
    public class ConfigTests
    {
        private string[] optionalFrom = new string[] { "Filter", "SubDirectories", "InitialScan", "Ignore", "IgnoreStrings" };

        [Test]
        public void FromDefaultsWork()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                { "Target", "/test" },
                { "Notifications", "Added" }
            };

            Dictionary<string, string> resolved = headers.ToFromConfig();
            Assert.IsTrue(this.ValidateFromHeaders(resolved));
        }


        private bool ValidateFromHeaders(Dictionary<string, string> resolved)
        {
            bool response = true;

            if(resolved.Count != 7)
            {
                return false;
            }

            for(int i = 0; i < optionalFrom.Length; i++)
            {
                if(!resolved.ContainsKey(optionalFrom[i]))
                {
                    response = false;
                    break;
                }
            }

            return response;
        }
    }
}
