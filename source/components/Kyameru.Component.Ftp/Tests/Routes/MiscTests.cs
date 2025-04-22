using Kyameru.Component.Ftp.Extensions;
using Xunit;

namespace Kyameru.Component.Ftp.Tests.Routes;

public class MiscTests
{
    [Fact]
    public void StringisNullReurnsTrue()
    {
        Assert.True(string.Empty.IsNullOrEmptyPath());
    }

    [Theory]
    [InlineData("path/", "path")]
    [InlineData("path", "path")]
    public void StringStripsPath(string input, string expected)
    {
        Assert.Equal(expected, input.StripEndingSlash());
    }
}
