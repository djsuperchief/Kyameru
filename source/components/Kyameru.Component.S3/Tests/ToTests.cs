using Amazon.S3;
using Kyameru.Component.S3.Exceptions;
using NSubstitute;

namespace Kyameru.Component.S3.Tests;

public class ToTests
{
    [Fact]
    public void HostHeaderIsRequired()
    {
        var s3Client = Substitute.For<IAmazonS3>();
        var to = new S3To(s3Client);
        Assert.Throws<MissingHeaderException>(() =>
        {
            to.SetHeaders(new Dictionary<string, string>());
        });
    }
}
