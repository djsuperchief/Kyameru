using Amazon.S3;
using Kyameru.Core.Entities;

namespace Kyameru.Component.S3.Tests;

public class S3FileTargetTests
{
    [Theory]
    [InlineData("STANDARD")]
    [InlineData("DEEP_ARCHIVE")]
    [InlineData("GLACIER")]
    [InlineData("GLACIER_IR")]
    [InlineData("INTELLIGENT_TIERING")]
    [InlineData("ONEZONE_IA")]
    [InlineData("OUTPOSTS")]
    [InlineData("REDUCED_REDUNDANCY")]
    [InlineData("STANDARD_IA")]
    [InlineData("SNOW")]
    [InlineData("EXPRESS_ONEZONE")]
    public void StorageTypeIsCorrect(string storageClass)
    {
        var routable = GetBaseRoutable();
        routable.SetHeader("S3StorageClass", storageClass);
        var response = S3FileTarget.FromRoutable(routable, "", "", "mybucket");
        var uploadObject = response.ToPutObjectRequest();
        Assert.Equal(storageClass, uploadObject.StorageClass.Value);
    }

    [Fact]
    public void InvalidStorageClassFails()
    {
        var routable = GetBaseRoutable();
        routable.SetHeader("S3StorageClass", "Banana");

        Assert.Throws<Exceptions.S3PropertyException>(
            () => S3FileTarget.FromRoutable(routable, "", "", "mybucket"));
    }

    [Fact]
    public void DefaultHeadersAreFileProperties()
    {
        var headers = new Dictionary<string, string>()
        {
            { "S3ContentType", "text/plain" },
            { "S3DataType", "String" }
        };
        var routable = new Routable(headers, "Test");
        var response = S3FileTarget.FromRoutable(routable, "/Path", "FileName.txt", "mybucket");
        Assert.Equal("/Path/", response.Path);
        Assert.Equal("FileName.txt", response.FileName);
        Assert.Equal("mybucket", response.Bucket);
    }

    [Theory]
    [InlineData("/Path")]
    [InlineData("/Path/")]
    public void PathAlteredCorrectly(string path)
    {
        var headers = new Dictionary<string, string>()
        {
            { "S3ContentType", "text/plain" },
            { "S3DataType", "String" }
        };
        var routable = new Routable(headers, "Test");
        var response = S3FileTarget.FromRoutable(routable, path, "FileName.txt", "mybucket");
        Assert.Equal("/Path/", response.Path);
    }

    [Fact]
    public void MessageOverridesTakePrecedence()
    {
        var routable = GetBaseRoutable();
        var response = S3FileTarget.FromRoutable(routable, "/Path", "FileName.txt", "mybucket");
        Assert.Equal("/test/", response.Path);
        Assert.Equal("Test.txt", response.FileName);
        Assert.Equal("mybucket", response.Bucket);
    }

    [Theory]
    [InlineData("File", S3FileTarget.OperationType.File, false)]
    [InlineData("String", S3FileTarget.OperationType.String, false)]
    [InlineData("Byte", S3FileTarget.OperationType.Byte, false)]
    [InlineData("Banana", null, true)]
    public void UploadTypeIsCorrect(string uploadType, S3FileTarget.OperationType? expectedOperation, bool throwsException)
    {
        var routable = GetBaseRoutable();
        routable.SetHeader("S3DataType", uploadType);
        if (uploadType == "File")
        {
            routable.SetHeader("FullSource", "/var/test");
        }

        if (!throwsException)
        {
            var response = S3FileTarget.FromRoutable(routable, "/Path", "FileName.txt", "mybucket");
            Assert.Equal(expectedOperation, response.UploadType);
        }
        else
        {
            Assert.Throws<ArgumentException>(() => S3FileTarget.FromRoutable(routable, "/Path", "FileName.txt", "mybucket"));
        }

    }

    [Fact]
    public void UploadFileRequestFailsWithNoFullSource()
    {
        var routable = GetBaseRoutable();
        routable.SetHeader("S3DataType", "File");
        Assert.Throws<Exceptions.MissingHeaderException>(() => S3FileTarget.FromRoutable(routable, "/Path", "FileName.txt", "mybucket"));
    }

    [Fact]
    public void PutRequestIsAsExpected_File()
    {
        var routable = GetBaseRoutable();
        routable.SetHeader("S3DataType", "File");
        routable.SetHeader("FullSource", "/var/test.data");
        var fileTarget = S3FileTarget.FromRoutable(routable, "/Path", "FileName.txt", "mybucket");
        var putRequest = fileTarget.ToPutObjectRequest();
        Assert.Equal("mybucket", putRequest.BucketName);
        Assert.Equal("/test/Test.txt", putRequest.Key);
        Assert.Equal("/var/test.data", putRequest.FilePath);
    }

    [Fact]
    public void PutRequestIsAsExpected_String()
    {
        var routable = GetBaseRoutable();
        routable.SetHeader("S3DataType", "String");
        routable.SetBody<string>("Hi there");
        var fileTarget = S3FileTarget.FromRoutable(routable, "/Path", "FileName.txt", "mybucket");
        var putRequest = fileTarget.ToPutObjectRequest();
        Assert.Equal("mybucket", putRequest.BucketName);
        Assert.Equal("/test/Test.txt", putRequest.Key);
        Assert.Null(putRequest.FilePath);
        Assert.Equal("Hi there", putRequest.ContentBody);
        Assert.Equal("text/plain", putRequest.ContentType);
    }

    [Fact]
    public void PutRequestIsAsExpected_Byte()
    {
        var routable = GetBaseRoutable();
        routable.SetHeader("S3DataType", "Byte");
        var fileTarget = S3FileTarget.FromRoutable(routable, "/Path", "FileName.txt", "mybucket");
        var putRequest = fileTarget.ToPutObjectRequest();
        Assert.Equal("mybucket", putRequest.BucketName);
        Assert.Equal("/test/Test.txt", putRequest.Key);
        Assert.Null(putRequest.FilePath);
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