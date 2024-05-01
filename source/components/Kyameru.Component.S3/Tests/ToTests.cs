using System.Text;
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
        var s3Client = GetAmazonS3();

        var to = new S3To(s3Client);
        var headers = new Dictionary<string, string>() {
            { "Host", "MyBucket" }
        };
        var routable = GetBaseRoutable();
        routable.SetHeader("S3DataType", "String");
        routable.SetHeader("FullSource", "/var/test.data");
        to.SetHeaders(headers);
        var exception = await Record.ExceptionAsync(() => to.ProcessAsync(routable, default));
        Assert.Null(exception);
        Assert.Equal("String", routable.Headers["S3ETag"]);
    }

    [Fact]
    public async Task CanProcessFilePath()
    {
        var s3Client = GetAmazonS3();

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
        Assert.Equal("FilePath", routable.Headers["S3ETag"]);
    }

    [Fact]
    public async Task CanProcessByte()
    {
        var s3Client = GetAmazonS3();

        var to = new S3To(s3Client);
        var headers = new Dictionary<string, string>() {
            { "Host", "MyBucket" }
        };
        var routable = GetBaseRoutable();
        routable.SetHeader("S3DataType", "Byte");
        routable.SetBody<Byte[]>(Encoding.UTF8.GetBytes("Hello world"));
        to.SetHeaders(headers);
        var exception = await Record.ExceptionAsync(() => to.ProcessAsync(routable, default));
        Assert.Null(exception);
        Assert.Equal("Byte", routable.Headers["S3ETag"]);
    }

    [Fact]
    public async Task CanProcessFromHeadersOnly()
    {
        var expected = "MyBucket";
        var s3Client = Substitute.For<IAmazonS3>();
        var received = string.Empty;
        s3Client.PutObjectAsync(Arg.Any<PutObjectRequest>(), Arg.Any<CancellationToken>()).Returns(x =>
        {
            var request = (PutObjectRequest)x[0];
            var response = new PutObjectResponse()
            {
                ETag = request.BucketName
            };
            received = request.BucketName;
            return response;
        });

        var to = new S3To(s3Client);
        var headers = new Dictionary<string, string>()
        {
            { "Host", expected },
            { "FileName", "MyFile.txt" }
        };
        var routable = new Routable(new Dictionary<string, string>()
        {
            { "S3DataType", "String" }
        }, "Test data");
        to.SetHeaders(headers);
        await to.ProcessAsync(routable, default);
        Assert.Equal(expected, received);
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

    private IAmazonS3 GetAmazonS3()
    {
        var s3Client = Substitute.For<IAmazonS3>();

        s3Client.PutObjectAsync(Arg.Any<PutObjectRequest>(), Arg.Any<CancellationToken>()).Returns(x =>
        {
            var request = (PutObjectRequest)x[0];
            var response = new PutObjectResponse()
            {
                ETag = "String"
            };

            if (!string.IsNullOrWhiteSpace(request.FilePath))
            {
                response.ETag = "FilePath";
            }

            if (request.InputStream != null)
            {
                response.ETag = "Byte";
            }

            return response;
        });

        return s3Client;
    }
}
