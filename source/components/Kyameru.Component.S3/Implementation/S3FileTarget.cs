using Amazon.S3;
using Amazon.S3.Model;
using Kyameru.Core.Entities;

namespace Kyameru.Component.S3;

public class S3FileTarget
{
    public string Path { get; private set; }

    public string FileName { get; private set; }

    public string ContentType { get; private set; }

    public string BodyString { get; private set; }

    public string Bucket { get; private set; }

    public bool Encrypt { get; private set; }

    public S3StorageClass StorageClass { get; private set; }

    public OperationType UploadType { get; private set; }

    public object MessageBody { get; private set; }

    public string FilePath { get; private set; }

    public enum OperationType
    {
        String,
        File,
        Byte
    }

    public static S3FileTarget FromRoutable(Routable item, string targetPath, string targetFile, string bucketName)
    {
        var response = new S3FileTarget
        {
            Path = item.Headers.TryGetValue("S3Path", targetPath),
            FileName = item.Headers.TryGetValue("S3FileName", targetFile),
            ContentType = item.Headers.TryGetValue("S3ContentType", "text/plain"),
            Bucket = bucketName,
            StorageClass = new S3StorageClass(item.Headers.TryGetValue("S3StorageClass", "STANDARD")),
            Encrypt = bool.Parse(item.Headers.TryGetValue("S3Encrypt", "false")),
            MessageBody = item.Body,
            FilePath = item.Headers.TryGetValue("FullSource", string.Empty)
        };

        if (!response.Path.EndsWith("/"))
        {
            response.Path += "/";
        }

        response.UploadType = Enum.Parse<OperationType>(item.Headers["S3DataType"]);
        response.MessageBody = item.Body;
        return response;
    }

    public PutObjectRequest ToPutObjectRequest()
    {
        var response = new PutObjectRequest()
        {
            BucketName = Bucket,
            Key = $"{Path}{FileName}",
        };

        if (UploadType == OperationType.String)
        {
            response.ContentBody = MessageBody.ToString();
        }

        if (UploadType == OperationType.File)
        {
            response.FilePath = FilePath;
        }

        return response;
    }
}