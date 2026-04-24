using System;

namespace Kyameru.Component.DynamoDB.Exceptions
{
    public class InvalidTypeException : Exception
    {
        public InvalidTypeException(string message) : base(message) { }
    }
}