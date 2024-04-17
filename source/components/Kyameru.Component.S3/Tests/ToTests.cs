using Amazon.S3;
using Amazon.S3.Model;
using Kyameru.Component.S3.Exceptions;
using Kyameru.Core.Entities;
using NSubstitute;

namespace Kyameru.Component.S3.Tests;

public class ToTests
{
    [Fact]
    public void HostHeaderIsRequiredNotSupplied()
    {
        var s3Client = Substitute.For<IAmazonS3>();
        var to = new S3To(s3Client);
        Assert.Throws<MissingHeaderException>(() =>
        {
            to.SetHeaders(new Dictionary<string, string>());
        });
    }

    [Fact]
    public void HostHeaderIsRequiredSuppliedBlank()
    {
        var s3Client = Substitute.For<IAmazonS3>();
        var to = new S3To(s3Client);
        Assert.Throws<MissingHeaderException>(() =>
        {
            to.SetHeaders(new Dictionary<string, string>()
            {
                { "Host", string.Empty}
            });
        });
    }

    [Fact]
    public async Task CanProcessString()
    {
        var s3Client = Substitute.For<IAmazonS3>();

        s3Client.PutObjectAsync(Arg.Any<PutObjectRequest>(), Arg.Any<CancellationToken>()).Returns(x =>
        {
            var request = (PutObjectRequest)x[0];
            var response = new PutObjectResponse()
            {
                VersionId = "Ok"
            };

            if (!string.IsNullOrWhiteSpace(request.FilePath))
            {
                response.VersionId = string.Empty;
            }

            return response;
        });

        var to = new S3To(s3Client);
        var headers = new Dictionary<string, string>() {
            { "Host", "MyBucket" }
        };
        var routable = GetBaseRoutable();
        routable.SetHeader("S3DataType", "File");
        routable.SetHeader("FullSource", "/var/test.data");
        to.SetHeaders(headers);
        var exception = await Record.ExceptionAsync(() => to.ProcessAsync(routable, default));
        Assert.Null(exception);
    }

    private Routable GetBaseRoutable()
    {
        var headers = new Dictionary<string, string>()
        {
            { "S3Path", "/test" },
            { "S3FileName", "Test.txt" },
            { "S3ContentType", "text/plain" },
            { "S3DataType", "String" }
        };

        return new Routable(headers, "Test");
    }
}
