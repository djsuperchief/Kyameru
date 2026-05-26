namespace Kyameru.Component.DynamoDB
{
    public class Resources
    {
        public const string EXCEPTION_ROUTABLENOTCOMPATIBLE = "Routable is not a compatible DynamoDB table record";
        public const string EXCEPTION_ROUTABLEEMPTY = "Routable is empty";
        public const string EXCEPTION_INVALIDBATCHSIZE_SMALL = "Invalid batch size, must be more than or equal to 1";
        public const string EXCEPTION_INVALIDBATCHSIZE_LARGE = "Invalid batch size, must be less than or equal to 25";
        public const string EXCEPTION_MISSINGTABLENAME = "Missing DynamoDB table name";
        
        public const string WARNING_NOENTITIES = "DynamoDb entities empty, no action to perform";

        public const string INFO_PROCESSING = "Processing routable via DynamoDB";
        public const string DEBUG_VALIDATINGROUTABLE = "Validating routable";
        public const string INFO_PROCESSINGSINGLE = "Processing single record to DynamoDB";
        public const string INFO_PROCESSINGMULTIPLE = "Processing multiple records to DynamoDB";
    }
}