namespace Kyameru.Component.Sns
{
    internal class Resources
    {
        public const string MISSING_HEADER_EXCEPTION = "One or more required headers is missing. \n{0}";
        public const string ERROR_HTTP_RESPONSE = "Error sending SNS message. '{0}' status does not indicate success.";
        public const string ERROR_SENDING = "Error sending SNS message: {0}";

        public const string INFO_PROCESSING = "Processing message for SNS Send...";
        public const string INFO_SENDING = "Sending SNS message...";

        public const string INFO_SENT = "SNS message sent with Id '{0}'";
    }
}