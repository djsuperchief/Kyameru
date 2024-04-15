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