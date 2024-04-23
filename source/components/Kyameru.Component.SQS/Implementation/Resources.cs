namespace Kyameru.Component.Sqs;

public class Resources
{
    public const string MISSING_HEADER_EXCEPTION = "Required header missing: {0}.";
    public const string MESSAGE_SENDING_EXCEPTION = "Failed to send SQS message to queue {0}: {1}";

    public const string INFORMATION_SEND = "Sending message to SQS queue {0}";
    public const string INFORMATION_SENT = "SQS Message sent, message id: {0}";
}