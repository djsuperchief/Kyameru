namespace Kyameru.Component.S3.Exceptions;

public class MissingHeaderException : Exception
{
    public MissingHeaderException(string message) : base(message)
    {

    }
}
