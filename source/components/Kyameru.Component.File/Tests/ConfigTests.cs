using System.Collections.Generic;
using Xunit;

namespace Kyameru.Component.File.Tests;

public class ConfigTests
{
    private string[] optionalFrom = new string[] { "Filter", "SubDirectories", "InitialScan", "Ignore", "IgnoreStrings" };

    [Fact]
    public void FromDefaultsWork()
    {
        var headers = new Dictionary<string, string>()
        {
            { "Target", "/test" },
            { "Notifications", "Added" }
        };

        var resolved = headers.ToFromConfig();
        Assert.True(this.ValidateFromHeaders(resolved));
    }


    private bool ValidateFromHeaders(Dictionary<string, string> resolved)
    {
        var response = true;

        if (resolved.Count != 7)
        {
            return false;
        }

        for (int i = 0; i < optionalFrom.Length; i++)
        {
            if (!resolved.ContainsKey(optionalFrom[i]))
            {
                response = false;
                break;
            }
        }

        return response;
    }
}
