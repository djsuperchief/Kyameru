namespace Kyameru.Component.Sqs;

public class Resources
{
    public const string MISSING_HEADER_EXCEPTION = "Required header missing: {0}.";
    public const string MESSAGE_SENDING_EXCEPTION = "Failed to send SQS message to queue {0}: {1}";

    public const string SQS_QUEUE_SCAN_EXCEPTION = "Error fetching messages from queue {0}: {1}";

    public const string INFORMATION_SEND = "Sending message to SQS queue {0}";
    public const string INFORMATION_SENT = "SQS Message sent, message id: {0}";
    public const string INFORMATION_SCANSTART = "Starting SQS queue scanning on queue {0}.";
    public const string INFORMATION_MESSAGE_RECEIVED = "Received message from SQS.";

    public const string INFORMATION_SCANNING = "Scanning for messages...";

    public const string INFORMATION_PROCESSING_RECEIVED = "Processing SQS message with id: {0}";
}