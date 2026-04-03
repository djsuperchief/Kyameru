using Kyameru.Core.Entities;

namespace Kyameru.Component.Sns.Tests;

public class SnsMessageTests
{
    [Fact]
    public void SnsMessageCanBeConstructedFromRoutable()
    {
        var routable = new Routable(new Dictionary<string, string>(), "test");

        var output = SnsMessage.FromRoutable(routable, new Dictionary<string, string>() { { "ARN", "test:arn" } });
        Assert.NotNull(output);
        Assert.Equal("test", output.Message);
        Assert.Equal("test:arn", output.Arn);
        Assert.Empty(output.Attributes);
    }
}