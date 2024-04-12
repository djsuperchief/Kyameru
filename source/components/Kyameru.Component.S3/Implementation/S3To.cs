using System.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Kyameru.Component.S3.Exceptions;
using Kyameru.Core.Entities;

namespace Kyameru.Component.S3;

public class S3To : ITo
{
    private IAmazonS3 s3client;
    private string targetBucket;
    private string targetPath;
    private string targetFileName;
    private string targetContentType;

    private readonly Dictionary<S3FileTarget.OperationType, Func<S3FileTarget, CancellationToken, Task>> targetActions;

    public event EventHandler<Log> OnLog;

    public S3To(IAmazonS3 client)
    {
        // TODO: Inject S3 config so we can override endpoint etc.
        s3client = client;
        targetActions = new Dictionary<S3FileTarget.OperationType, Func<S3FileTarget, CancellationToken, Task>>()
        {
            { S3FileTarget.OperationType.String, UploadFile },
            { S3FileTarget.OperationType.Byte, UploadByteArray },
            { S3FileTarget.OperationType.File, UploadFile }

        };
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
            var s3TargetFile = S3FileTarget.FromRoutable(routable, targetPath, targetFileName, targetBucket);
            await targetActions[s3TargetFile.UploadType](s3TargetFile, cancellationToken);
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

        ValidateHeaders(headers);
    }

    private async Task UploadByteArray(S3FileTarget item, CancellationToken cancellationToken)
    {
        var request = item.ToPutObjectRequest();
        PutObjectResponse response;
        using(var memoryStream = new MemoryStream(item.MessageBody as byte[]))
        {
            
            request.InputStream = memoryStream;
            response = await s3client.PutObjectAsync(request, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(response.VersionId))
        {
            throw new UploadFailedException("Upload failed");
        }
    }

    private async Task UploadFile(S3FileTarget item, CancellationToken cancellationToken)
    {
        var response = await s3client.PutObjectAsync(item.ToPutObjectRequest(), cancellationToken);
        if (!string.IsNullOrWhiteSpace(response.VersionId))
        {
            throw new UploadFailedException("Upload failed");
        }
    }

    private void ValidateHeaders(Dictionary<string, string> headers)
    {
        if (string.IsNullOrWhiteSpace(headers["Host"]))
        {
            throw new Exceptions.MissingHeaderException(string.Format(Resources.ERROR_MISSINGHEADER, "Host"));
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