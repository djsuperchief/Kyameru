using System;

namespace Kyameru.Component.DynamoDB.Exceptions
{
    public class InvalidBatchSizeException : Exception
    {
        public InvalidBatchSizeException(string message) : base(message) { }
    }
}