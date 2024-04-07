﻿using Amazon.S3;
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
            Path = item.Headers.TryGetValue("S3Path") ?? targetPath,
            FileName = item.Headers.TryGetValue("S3FileName") ?? targetFile,
            ContentType = item.Headers.TryGetValue("S3ContentType"),
            Bucket = bucketName,
            StorageClass = new S3StorageClass(item.Headers.TryGetValue("S3StorageClass", "STANDARD"))
        };

        if (response.Path.EndsWith("/"))
        {
            response.Path += "/";
        }

        response.UploadType = Enum.Parse<OperationType>(item.Headers["DataType"]);
        // TODO: Set body of file target (method)

        return response;
    }

    public PutObjectRequest ToPutObjectRequestString()
    {
        return new PutObjectRequest()
        {
            BucketName = Bucket,
            Key = $"{Path}{FileName}",
            ContentBody = BodyString,
            ContentType = ContentType ?? "text/plain"
        };
    }

    private static void SetResponseBody(S3FileTarget response)
    {

    }
}