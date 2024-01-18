using Kyameru.Core.Exceptions;

namespace Kyameru.Component.SQS.Tests;

public class ConfigTests
{
    [Fact]
    public void ConfigIsCreatecCorrectly()
    {
        var headers = new Dictionary<string, string>()
        {
            { "serviceurl", "Test1" },
            { "accesskey", "Test2" },
            { "secretkey", "Test3" },
            { "queue", "myqueue" }
        };

        var config = headers.ParseHeadersToAwsConfig();
        Assert.True(config.AccessKey == "Test2" &&
            config.SecretKey == "Test3" &&
            config.ServiceUrl == "Test1" &&
            config.Queue == "myqueue");
    }

    [Fact]
    public void RequiredHeadersMissingThrowsException()
    {
        var headers = new Dictionary<string, string>()
        {
            { "randomheader", "test"}
        };

        Assert.Throws<ActivationException>(() => headers.ParseHeadersToAwsConfig());
    }
}
