﻿using System.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Kyameru.Core.Entities;

namespace Kyameru.Component.S3;

public class S3To : ITo
{
    private IAmazonS3 s3client;
    private string targetBucket;
    private string targetPath;
    private string targetFileName;
    private string targetContentType;

    public event EventHandler<Log> OnLog;

    public S3To(IAmazonS3 client)
    {
        // TODO: Inject S3 config so we can override endpoint etc.
        s3client = client;
    }

    public void Process(Routable routable)
    {
        throw new NotImplementedException();
    }

    public async Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
    {
        try
        {
            ValidateS3Targets(routable);
            ValidateDataType(routable);
            if (routable.Headers["DataType"] == "String")
            {
                await UploadStringFile(routable, cancellationToken);
            }
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine(
                    "Error encountered ***. Message:'{0}' when writing an object"
                    , e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(
                "Unknown encountered on server. Message:'{0}' when writing an object"
                , e.Message);
        }
    }

    public void SetHeaders(Dictionary<string, string> headers)
    {
        targetBucket = headers["Host"];
        if (headers.TryGetValue("Target", out var pathHeader))
        {
            targetPath = pathHeader;
        }

        if (headers.TryGetValue("FileName", out var fileName))
        {
            targetFileName = fileName;
        }
    }

    private async Task UploadByteArray(Routable item, CancellationToken cancellationToken)
    {

    }

    private async Task UploadStringFile(Routable item, CancellationToken cancellationToken)
    {
        if (!targetPath.EndsWith("/"))
        {
            targetPath += "/";
        }

        var request = new PutObjectRequest
        {
            BucketName = targetBucket,
            Key = $"{targetPath}{targetFileName}",
            ContentBody = item.Body.ToString(),
            ContentType = targetContentType ?? "text/plain"
        };
        var response = await s3client.PutObjectAsync(request, cancellationToken);
        if (!string.IsNullOrWhiteSpace(response.VersionId))
        {
            // TODO: Proper exception.
            throw new NullReferenceException("Upload failed");
        }
    }

    private void ValidateHeaders(Dictionary<string, string> headers)
    {
        if (string.IsNullOrWhiteSpace(headers["Host"]))
        {
            throw new Exceptions.MissingHeaderException(string.Format(Resources.ERROR_MISSINGHEADER, "Target"));
        }
    }

    private void ValidateS3Targets(Routable item)
    {
        targetPath ??= item.Headers.TryGetValue("S3Path");
        targetFileName ??= item.Headers.TryGetValue("S3FileName");
        targetContentType ??= item.Headers.TryGetValue("S3ContentType");

        if (string.IsNullOrWhiteSpace(targetPath))
        {
            throw new Exceptions.MissingHeaderException(string.Format(Resources.ERROR_MISSINGHEADER, "S3Path"));
        }

        if (string.IsNullOrWhiteSpace(targetFileName))
        {
            throw new Exceptions.MissingHeaderException(string.Format(Resources.ERROR_MISSINGHEADER, "S3FileName"));
        }
    }

    private void ValidateDataType(Routable item)
    {
        var dataType = item.Headers["DataType"];
        if (dataType != "String"
        && dataType != "Byte")
        {
            throw new Exceptions.InvalidDataTypeException(Resources.ERROR_INVALIDDATATYPE);
        }
    }
}
