namespace Kyameru.Component.S3.Exceptions;

public class UploadFailedException : Exception
{
    public UploadFailedException(string message) : base(message)
    {
    }
}