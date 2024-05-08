using Amazon.S3;
using Amazon.S3.Model;
using Kyameru.Core.Entities;

namespace Kyameru.Component.S3;

public class S3FileTarget
{
    public string? Path { get; private set; }

    public string? FileName { get; private set; }

    public string? ContentType { get; private set; }

    public string? Bucket { get; private set; }

    public bool Encrypt { get; private set; }

    public S3StorageClass? StorageClass { get; private set; }

    public OperationType UploadType { get; private set; }

    public object? MessageBody { get; private set; }

    public string? FilePath { get; private set; }

    public string Key => string.IsNullOrWhiteSpace(Path) || Path == "/" ? FileName : $"{Path}{FileName}";

    public enum OperationType
    {
        String,
        File,
        Byte
    }



    public static S3FileTarget FromRoutable(Routable item, string targetPath, string targetFile, string bucketName)
    {
        var storageClass = item.Headers.TryGetValue("S3StorageClass", "STANDARD").ToUpper();
        ValidateStorageTypes(storageClass);
        var response = new S3FileTarget
        {
            Path = item.Headers.TryGetValue("S3Path", targetPath),
            FileName = item.Headers.TryGetValue("S3FileName", targetFile),
            ContentType = item.Headers.TryGetValue("S3ContentType", "text/plain"),
            Bucket = bucketName,
            StorageClass = new S3StorageClass(storageClass),
            Encrypt = bool.Parse(item.Headers.TryGetValue("S3Encrypt", "false")),
            MessageBody = item.Body,
            FilePath = item.Headers.TryGetValue("FullSource", string.Empty)
        };

        response.Path ??= string.Empty;
        if (!response.Path.EndsWith("/"))
        {
            response.Path += "/";
        }

        response.UploadType = Enum.Parse<OperationType>(item.Headers["S3DataType"]);
        if (response.UploadType == OperationType.File && string.IsNullOrWhiteSpace(response.FilePath))
        {
            throw new Exceptions.MissingHeaderException(string.Format(Resources.ERROR_MISSINGHEADER, "FullSource"));
        }

        response.MessageBody = item.Body;
        return response;
    }

    private static void ValidateStorageTypes(string storageClass)
    {
        var validStorageTypes = new[]
        {
            "STANDARD",
            "DEEP_ARCHIVE",
            "GLACIER",
            "GLACIER_IR",
            "INTELLIGENT_TIERING",
            "ONEZONE_IA",
            "OUTPOSTS",
            "REDUCED_REDUNDANCY",
            "STANDARD_IA",
            "SNOW",
            "EXPRESS_ONEZONE"
        };

        if (!validStorageTypes.Contains(storageClass))
        {
            throw new Exceptions.S3PropertyException(string.Format(Resources.ERROR_INVALIDSTORAGECLASS, storageClass));
        }
    }

    public PutObjectRequest ToPutObjectRequest()
    {
        var response = new PutObjectRequest()
        {
            BucketName = Bucket,
            Key = $"{Path}{FileName}",
            StorageClass = StorageClass,
            ContentType = ContentType
        };

        if (UploadType == OperationType.String)
        {
            response.ContentBody = MessageBody?.ToString();
        }

        if (UploadType == OperationType.File)
        {
            response.FilePath = FilePath;
        }

        return response;
    }
}