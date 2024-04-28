using System.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Kyameru.Component.S3.Exceptions;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;

namespace Kyameru.Component.S3;

public class S3To : ITo
{
    private IAmazonS3 s3client;
    private string targetBucket = "";
    private string targetPath = "";
    private string targetFileName = "";
    private string targetContentType = "";

    private readonly Dictionary<S3FileTarget.OperationType, Func<S3FileTarget, CancellationToken, Task<string>>> targetActions;

    public event EventHandler<Log>? OnLog;

    public S3To(IAmazonS3 client)
    {
        // TODO: Inject S3 config so we can override endpoint etc.
        s3client = client;
        targetActions = new Dictionary<S3FileTarget.OperationType, Func<S3FileTarget, CancellationToken, Task<string>>>()
        {
            { S3FileTarget.OperationType.String, UploadFile },
            { S3FileTarget.OperationType.Byte, UploadByteArray },
            { S3FileTarget.OperationType.File, UploadFile }

        };
    }

    public void Process(Routable routable)
    {
        var tokenSource = new CancellationTokenSource();
        var task = Task.Factory.StartNew(() => ProcessAsync(routable, tokenSource.Token), tokenSource.Token);
        task.Wait(tokenSource.Token);
    }

    public async Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
    {
        try
        {
            ValidateS3Targets(routable);
            ValidateDataType(routable);
            var s3TargetFile = S3FileTarget.FromRoutable(routable, targetPath, targetFileName, targetBucket);
            var response = await targetActions[s3TargetFile.UploadType](s3TargetFile, cancellationToken);
            routable.SetHeader("&S3ETag", response);
            Log(LogLevel.Information, $"File uploaded with ETag {response}");
        }
        catch (AmazonS3Exception e)
        {
            Log(
                LogLevel.Error,
                $"Error encountered ***. Message:'{e.Message}' when writing an object",
                e);
        }
        catch (Exception e)
        {
            Log(
                LogLevel.Error,
                $"Unknown encountered on server. Message:'{e.Message}' when writing an object",
                e);
        }
    }

    public void SetHeaders(Dictionary<string, string> headers)
    {
        ValidateHeaders(headers);
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

    private async Task<string> UploadByteArray(S3FileTarget item, CancellationToken cancellationToken)
    {
        Log(LogLevel.Information, "Uploading byte array to bucket");
        var request = item.ToPutObjectRequest();
        PutObjectResponse response;
        using (var memoryStream = new MemoryStream(item.MessageBody as byte[] ?? Array.Empty<byte>()))
        {

            request.InputStream = memoryStream;
            response = await s3client.PutObjectAsync(request, cancellationToken);
        }

        if (string.IsNullOrWhiteSpace(response.ETag))
        {
            throw new UploadFailedException("Upload failed");
        }

        return response.ETag;
    }

    private async Task<string> UploadFile(S3FileTarget item, CancellationToken cancellationToken)
    {
        Log(LogLevel.Information, "Uploading string to bucket");
        var response = await s3client.PutObjectAsync(item.ToPutObjectRequest(), cancellationToken);
        if (string.IsNullOrWhiteSpace(response.ETag))
        {
            throw new UploadFailedException("Upload failed");
        }

        return response.ETag;
    }

    private void ValidateHeaders(Dictionary<string, string> headers)
    {
        if (!headers.ContainsKey("Host") || string.IsNullOrWhiteSpace(headers["Host"]))
        {
            throw new Exceptions.MissingHeaderException(string.Format(Resources.ERROR_MISSINGHEADER, "Host"));
        }
    }

    private void ValidateS3Targets(Routable item)
    {
        Log(LogLevel.Information, "Validating S3 Targets");
        targetPath = string.IsNullOrWhiteSpace(targetPath) ? item.Headers.TryGetValue("S3Path") : string.Empty;
        targetFileName = string.IsNullOrWhiteSpace(targetFileName) ? item.Headers.TryGetValue("S3FileName") : string.Empty;
        targetContentType = string.IsNullOrWhiteSpace(targetContentType) ? item.Headers.TryGetValue("S3ContentType") : string.Empty;

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
        Log(LogLevel.Information, "Validating Data Type");
        var dataType = item.Headers["S3DataType"];
        if (dataType != "String"
        && dataType != "Byte"
        && dataType != "File")
        {
            throw new Exceptions.InvalidDataTypeException(Resources.ERROR_INVALIDDATATYPE);
        }
    }

    private void Log(LogLevel logLevel, string message, Exception? exception = null)
    {
        this.OnLog?.Invoke(this, new Core.Entities.Log(logLevel, message, exception));
    }
}